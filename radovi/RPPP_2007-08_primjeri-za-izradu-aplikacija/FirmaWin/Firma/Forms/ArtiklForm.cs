using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

using NTier;

namespace Firma
{
  // Forma artikala
  public partial class ArtiklForm : BaseForm
  {
    #region Constructors & Init
    public ArtiklForm()
    {
      InitializeComponent();

      // Dohvat svih Artikala
      artiklBindingSource.DataSource = artiklBll.FetchAll();

      // Postavljanje stanja forme
      State = BusinessObjectState.Unmodified;

      // Povezivanje na objekte - programski radi definiranja formata prikaza
      cijArtiklaTextBox.DataBindings.Add(
        new Binding("Text", artiklBindingSource, "CijArtikla", // standardno
                    true,                         // formatiranje omogu�eno
                    DataSourceUpdateMode.OnPropertyChanged, // a�uriranje izvora
                    string.Empty, "N2")); // vrijednost za null, format
    }
    #endregion

    #region BLL Objects
    // BLL sloj
    private ArtiklBllProvider artiklBll = new ArtiklBllProvider();

    // Metoda preko koje �e poslovni objekt do�i do BLL sloja kako bi obavio validaciju unosa
    private IBllObject Artikl_NeedBllObject()
    {
      return artiklBll;
    }
    #endregion

    #region Artikl Changed
    // Osvje�avanje unosa i pridru�ivanje metode za BLL objekt poslovnom objektu
    private void artiklBindingSource_CurrentChanged(object sender, EventArgs e)
    {
      // as za razliku od obi�ne pretvorbe tipa ne�e puknuti s InvalidCastException
      // ako tip nije ispravan nego �e vratiti null.
      Artikl a = artiklBindingSource.Current as Artikl;

      // Ako se radi o novom poslovnom objektu 
      // koji jo� ne zna gdje mu je BLL objekt... a�uriraj.
      if (a != null && !a.HasBllObject)
      {
        a.NeedBllObject += new NeedBllObjectEventHandler(Artikl_NeedBllObject);
      }
    }

    // postavljanje naslova forme kad se promijeni aktualni artikl  
    private void artiklBindingSource_CurrentItemChanged(object sender, EventArgs e)
    {
      this.Text = "Artikl";
      if (artiklBindingSource.Current != null)
      {
        this.Text = "Artikl: " + ((Artikl)artiklBindingSource.Current).ToString();
      }
    }
    #endregion

    #region Pona�anje forme i kontrola
    // Obri�i sve oznake za pogre�ku
    private void ClearError()
    {
      artiklErrorProvider.Clear();
      StatusBar.IsError = false;
      StatusBar.Message = string.Empty;
    }

    // Izvodi se nakon �to forma u�e u stanje izmjene
    protected override void AfterEdit()
    {
      base.AfterEdit();
      ((Artikl)artiklBindingSource.Current).ClearErrors();
    }

    // Izvodi se nakon �to forma u�e u stanje novog unosa
    protected override void AfterNew()
    {
      base.AfterNew();
      ((Artikl)artiklBindingSource.Current).ClearErrors();
    }

    // Osvje�avanje kontrola
    private void ArtiklForm_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName.Equals("InEditMode"))
      {
        artiklPanel.Enabled = InEditMode;
        btnSetSlikaArtikla.Enabled = btnClearSlikaArtikla.Enabled =
            btnSetSlikaArtikla.Visible = btnClearSlikaArtikla.Visible = toolStripSeparator2.Visible = InEditMode;

        if (!InEditMode)
        {
          slikaArtiklaPictureBox.ContextMenuStrip = null;
        }
        else
        {
          slikaArtiklaPictureBox.ContextMenuStrip = contextMenuStrip;
        }
      }
      else if (e.PropertyName.Equals("State"))
      {
        sifArtiklaTextBox.Enabled = State == BusinessObjectState.New;
      }
    }

    // Ako nema pogre�ke o�isti kontrolu, ina�e...
    private void ArtiklControlEnter(object sender, EventArgs e)
    {
      if (InEditMode)
      {
        string error = artiklErrorProvider.GetError(sender as Control);
        StatusBar.IsError = !string.IsNullOrEmpty(error);
        StatusBar.Message = error;
      }
      else
      {
        ClearError();
      }
    }

    private void ArtiklControlLeave(object sender, EventArgs e)
    {
      // Problem osnovnih .NET kontrola s NULL vrijednostima 
      // ... brisanjem teksta ne�e se postaviti NULL odnosno string.Empty u odre�eni property
      // Ova metoda zaobilazi ovaj nedostatak, ina�e rije�en u ve�ini 3rd party komponentama

      if (artiklBindingSource.Current != null)
      {
        if (sender is TextBox && string.IsNullOrEmpty((sender as TextBox).Text))
        {
          Utils.SetNull(sender as Control, "Text", artiklBindingSource.Current);
        }
      }

      StatusBar.IsError = false;
      StatusBar.Message = string.Empty;
    }
    #endregion

    #region Rad sa slikom
    private void btnSetSlikaArtikla_Click(object sender, EventArgs e)
    {
      if (InEditMode)
      {
        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
          using (FileStream f = new FileStream(openFileDialog.FileName, FileMode.Open))
          {
            byte[] buffer = new byte[f.Length];
            f.Read(buffer, 0, buffer.Length);
            ((Artikl)artiklBindingSource.Current).SlikaArtikla = buffer;
          }
        }
      }
    }

    private void btnClearSlikaArtikla_Click(object sender, EventArgs e)
    {
      if (InEditMode)
      {
        ((Artikl)artiklBindingSource.Current).SlikaArtikla = null;
      }
    }
    #endregion

    #region Spremanje & brisanje
    // Spremanje svih izmjena
    protected override void DoSaveChanges()
    {
      artiklBll.SaveChanges(((ArtiklList)artiklBindingSource.DataSource).GetChanges());
    }

    // Brisanje
    protected override void DoDelete()
    {
      // Uklanjanje poslovnog objekta iz liste dohva�enih objekata (ozna�ava objekt obrisanim)
      artiklBindingSource.RemoveCurrent();
      try
      {
        // Sprema izmjene u bazu. Objekti ozna�eni za brisanje biti �e uklonjeni iz baze.
        artiklBll.SaveChanges(((ArtiklList)artiklBindingSource.DataSource).GetChanges());
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message, "Neuspje�no brisanje!");

      }
    }
    #endregion

  }
}