using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;

using NTier;

namespace Firma
{
  // Wizard forma dokumenta
  public partial class DokumentWizardForm : WizardBaseForm
  {
    #region Vars
    private Dokument dok;
    #endregion

    #region Constructors & Init
    public DokumentWizardForm()
    {
      using (new StatusBusy())
      {
        InitializeComponent();

        artiklBindingSource.DataSource = artiklBll.FetchAll();

        dok = new Dokument();

        dokumentBindingSource.DataSource = dok;
        prethDokumentBindingSource.DataSource = dokumentBll.FetchAll();

        postoPorezTextBox.DataBindings.Add(new Binding("Text", dokumentBindingSource, "PostoPorez", true, DataSourceUpdateMode.OnPropertyChanged, string.Empty, "p"));
        iznosDokumentaTextBox.DataBindings.Add(new Binding("Text", dokumentBindingSource, "IznosDokumenta", true, DataSourceUpdateMode.OnPropertyChanged, string.Empty, "C2"));
      }
    }

    public void SetPartner(Partner p)
    {
      partnerInfoBindingSource.DataSource = p;
      if (p != null)
      {
        dok.IdPartnera = p.IdPartnera;
      }
      else
      {
        dok.IdPartnera = null;
      }
    }
    #endregion

    #region BLL Objects
    // BLL sloj
    private DokumentBllProvider dokumentBll = new DokumentBllProvider();
    private PartnerBllProvider partnerBll = new PartnerBllProvider();
    private ArtiklBllProvider artiklBll = new ArtiklBllProvider();
    private StavkaBllProvider stavkaBll = new StavkaBllProvider();

    // Metoda preko koje �e poslovni objekt do�i do BLL sloja kako bi obavio validaciju unosa
    private IBllObject Dokument_NeedBllObject()
    {
      return dokumentBll;
    }

    // Metoda preko koje �e poslovni objekt do�i do BLL sloja kako bi obavio validaciju unosa
    private IBllObject Stavka_NeedBllObject()
    {
      return stavkaBll;
    }
    #endregion

    #region Dokument Changed
    // Osvje�avanje unosa i pridru�ivanje metode za BLL objekt poslovnom objektu
    private void dokumentBindingSource_CurrentChanged(object sender, EventArgs e)
    {
      // as za razliku od obi�ne pretvorbe tipa ne�e puknuti s InvalidCastException
      // ako tip nije ispravan nego �e vratiti null.
      Dokument d = dokumentBindingSource.Current as Dokument;

      // Ako se radi o novom poslovnom objektu koji jo� ne zna gdje mu
      // je BLL objekt... a�uriraj.
      if (d != null)
      {
        if (!d.HasBllObject)
          d.NeedBllObject += new NeedBllObjectEventHandler(Dokument_NeedBllObject);

        // Drugi na�in bi bio staviti u Load forme (umjesto sljede�eg retka)
        // DataSource na dokumentBindingsource a DataMember na Stavke
        // Nedostatak takvog pristupa je mogu�nost dodavanja praznog retka
        // u grid (CancelEdit ne funkcionira kako bi trebao).
        stavkaBindingSource.DataSource = d.Stavke;
      }
    }

    private void dokumentBindingSource_CurrentItemChanged(object sender, EventArgs e)
    {
      this.Text = "Dokument";
      if (dokumentBindingSource.Current != null)
      {
        this.Text = ((Dokument)dokumentBindingSource.Current).ToString();
      }
    }
    #endregion

    #region Stavka Changed
    // Osvje�avanje unosa i pridru�ivanje metode za BLL objekt poslovnom objektu
    private void stavkaBindingSource_CurrentChanged(object sender, EventArgs e)
    {
      // as za razliku od obi�ne pretvorbe tipa ne�e puknuti s InvalidCastException
      // ako tip nije ispravan nego �e vratiti null.
      Stavka s = stavkaBindingSource.Current as Stavka;

      // Ako se radi o novom poslovnom objektu koji jo� ne zna gdje mu
      // je BLL objekt... a�uriraj.
      if (s != null && !s.HasBllObject)
      {
        s.NeedBllObject += new NeedBllObjectEventHandler(Stavka_NeedBllObject);
      }
    }
    #endregion

    #region Pona�anje forme
    // Obri�i sve oznake za pogre�ku
    private void ClearError()
    {
      dokumentErrorProvider.Clear();
      stavkaErrorProvider.Clear();
    }

    private void DokumentControlLeave(object sender, EventArgs e)
    {
      // Sa NULL vrijednostima osnovne .NET kontrole imaju problema.
      // Problem je �to se brisanjem teksta ne�e postaviti NULL odnosno string.Empty o odre�eni property

      // Ova metoda zaobilazi ovaj nedostatak koji je ina�e rije�en u ve�ini 3rd party komponentama

      if (dokumentBindingSource.Current != null)
      {
        if (sender is TextBox && string.IsNullOrEmpty((sender as TextBox).Text))
        {
          Utils.SetNull(sender as Control, "Text", dokumentBindingSource.Current);
        }
      }
    }

    protected void Zoom()
    {
      if (stavkaDataGridView.SelectedCells.Count == 1 && stavkaDataGridView.SelectedCells[0].ColumnIndex == 0 || stavkaDataGridView.SelectedCells[0].ColumnIndex == 1)
      {
        ArtiklForm f = new ArtiklForm();
        f.StartPosition = FormStartPosition.CenterScreen;
        if (f.ShowDialog() == DialogResult.OK)
        {
          // Potrebno zbog bindinga na lookup u slu�aju novounesenih artikala
          artiklBindingSource.DataSource = artiklBll.FetchAll();

          Artikl a = (Artikl)f.Selected;
          Stavka s = null;
          if (stavkaDataGridView.Rows[stavkaDataGridView.SelectedCells[0].RowIndex].IsNewRow)
          {
            stavkaBindingSource.CancelEdit();
            Dokument dok = (Dokument)dokumentBindingSource.Current;
            s = new Stavka();
            dok.Stavke.Add(s);
          }
          else
          {
            s = (Stavka)stavkaBindingSource.Current;
          }

          if (s != null)
          {
            s.SifArtikla = a.SifArtikla;
            s.NazArtikla = a.NazArtikla;
            s.JedMjereArtikla = a.JedMjere;
            s.JedCijArtikla = a.CijArtikla;
          }
        }
      }
    }

    // Osvje�avanje svojstava stavke. Ovo bi se ina�e trebalo raditi u poslovnom
    // objektu stavke ali to u ovom primjeru nije mogu�e zbog jednostavnijeg dizajna frameworka
    private void stavkaDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
    {
      if (e.RowIndex == -1)
        return;

      if (e.ColumnIndex == 0 || e.ColumnIndex == 1)
      {
        try
        {
          // �ifra
          object sifArtikla = stavkaDataGridView[e.ColumnIndex, e.RowIndex].Value;
          if (sifArtikla != null)
          {
            Artikl a = artiklBll.Fetch((int)sifArtikla);
            if (a != null)
            {
              Stavka s = stavkaBindingSource.Current as Stavka;
              s.JedCijArtikla = a.CijArtikla;
              s.JedMjereArtikla = a.JedMjere;
            }
          }
        }
        catch (InvalidCastException)
        {
          // Nije unesena ispravna vrijednost. Zanemari. Validacija �e to uloviti.
        }
      }
    }

    private void DokumentWizardForm_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.F5)
      {
        e.Handled = true;
        Zoom();
      }
    }
    #endregion

    #region Wizard Actions
    private void buttonCancel_Click(object sender, EventArgs e)
    {
      result = WizardResult.Cancel;
      this.Close();
    }

    private void buttonPrevious_Click(object sender, EventArgs e)
    {
      result = WizardResult.Previous;
      this.Close();
    }

    private void buttonNext_Click_1(object sender, EventArgs e)
    {
      try
      {
        List<Dokument> izmjene = new List<Dokument>();
        izmjene.Add(dok);
        dokumentBll.SaveChanges(izmjene);

        result = WizardResult.Finish;
        this.Close();
      }
      catch (Exception err)
      {
        MessageBox.Show(err.Message, "Pogre�ka", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }
    #endregion

    #region Business Object
    public Dokument Dokument
    {
      get
      {
        if (dokumentBindingSource != null && dokumentBindingSource.Current != null)
        {
          return dokumentBindingSource.Current as Dokument;
        }
        else
        {
          return null;
        }
      }
    }
    #endregion

    private void DokumentWizardForm_Activated(object sender, EventArgs e)
    {
      buttonNext.Left = 626;
      buttonPrevious.Left = 518;
      buttonCancel.Left = 382;
    }
  }
}