using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Reflection;

using NTier;

namespace Firma
{
  // Forma poslovnih partnera
  public partial class PartnerForm : BaseForm
  {
    #region Constructors & Init
    public PartnerForm()
    {
      InitializeComponent();

      // Dohvat svih partnera
      partnerBindingSource.DataSource = partnerBll.FetchAll();

      // Dohvat svih mjesta kako bi se omogu�io lookup
      MjestoList mjesta = (new MjestoBllProvider()).FetchAll();
      sjedisteMjestoBindingSource.DataSource = mjesta;
      isporukaMjestoBindingSource.DataSource = mjesta;

      // Postavljanje stanja forme
      State = BusinessObjectState.Unmodified;

      // Povezivanje na objekte - programski radi definiranja formata prikaza
      // Prikazivanje/skrivanje panela ovisno o tvrtka/osoba svojstva Checked
      osobaPanel.DataBindings.Add(new Binding("Visible", osobaCheck, "Checked"));
      tvrtkaPanel.DataBindings.Add(new Binding("Visible", tvrtkaCheck, "Checked"));
    }
    #endregion

    #region BLL Objects
    // BLL sloj
    private PartnerBllProvider partnerBll = new PartnerBllProvider();

    // Metoda preko koje �e poslovni objekt do�i do BLL sloja kako bi obavio validaciju unosa
    private IBllObject Partner_NeedBllObject()
    {
      return partnerBll;
    }
    #endregion

    #region Partner Changed
    // Osvje�avanje unosa i pridru�ivanje metode za BLL objekt poslovnom objektu
    private void partnerBindingSource_CurrentChanged(object sender, EventArgs e)
    {
      // as za razliku od obi�ne pretvorbe tipa ne�e puknuti s InvalidCastException
      // ako tip nije ispravan nego �e vratiti null.
      Partner p = partnerBindingSource.Current as Partner;

      // Ako se radi o novom poslovnom objektu 
      // koji jo� ne zna gdje mu je BLL objekt... a�uriraj.
      if (p != null && !p.HasBllObject)
      {
        p.NeedBllObject += new NeedBllObjectEventHandler(Partner_NeedBllObject);
      }

      // Osvje�avanje izgleda forme
      if (p == null)
      {
        osobaCheck.Checked = false;
        tvrtkaCheck.Checked = false;
      }
      else if (p.TipPartnera == TipPartnera.Tvrtka)
      {
        osobaCheck.Checked = false;
        tvrtkaCheck.Checked = true;
      }
      else
      {
        osobaCheck.Checked = true;
        tvrtkaCheck.Checked = false;
      }
    }

    private void partnerBindingSource_CurrentItemChanged(object sender, EventArgs e)
    {
      this.Text = "Partner";
      if (partnerBindingSource.Current != null)
      {
        this.Text = "Partner: " + ((Partner)partnerBindingSource.Current).ToString();
      }
    }
    #endregion

    #region Pona�anje forme i kontrola
    // Obri�i sve oznake za pogre�ku
    private void ClearError()
    {
      partnerErrorProvider.Clear();
      StatusBar.IsError = false;
      StatusBar.Message = string.Empty;
    }

    // Izvodi se nakon �to forma u�e u stanje izmjene
    protected override void AfterEdit()
    {
      base.AfterEdit();
      ((Partner)partnerBindingSource.Current).ClearErrors();

      imeOsobeTextBox.Focus();
    }

    // Izvodi se nakon �to forma u�e u stanje novog unosa
    protected override void AfterNew()
    {
      base.AfterNew();
      ((Partner)partnerBindingSource.Current).ClearErrors();
      ((Partner)partnerBindingSource.Current).TipPartnera = TipPartnera.Osoba;

      imeOsobeTextBox.Focus();
    }

    // Osvje�avanje kontrola
    private void PartnerForm_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName.Equals("InEditMode"))
      {
        partnerPanel.Enabled = InEditMode;
      }
      else if (e.PropertyName.Equals("State"))
      {
        groupBox3.Enabled = matBrTvrtkeTextBox.Enabled = jMBGTextBox.Enabled = State == BusinessObjectState.New;
      }
    }

    // Ako nema pogre�ke o�isti kontrolu, ina�e...
    private void PartnerControlEnter(object sender, EventArgs e)
    {
      if (InEditMode)
      {
        string error = partnerErrorProvider.GetError(sender as Control);
        StatusBar.IsError = !string.IsNullOrEmpty(error);
        StatusBar.Message = error;
      }
      else
      {
        ClearError();
      }
    }

    private void PartnerControlLeave(object sender, EventArgs e)
    {
      // Sa NULL vrijednostima osnovne .NET kontrole imaju problema.
      // Problem je �to se brisanjem teksta ne�e postaviti NULL odnosno string.Empty o odre�eni property

      // Ova metoda zaobilazi ovaj nedostatak koji je ina�e rije�en u ve�ini 3rd party komponentama

      if (partnerBindingSource.Current != null)
      {
        if (sender is TextBox && string.IsNullOrEmpty((sender as TextBox).Text))
        {
          Utils.SetNull(sender as Control, "Text", partnerBindingSource.Current);
        }
        else if (sender is ComboBox && string.IsNullOrEmpty((sender as ComboBox).Text))
        {
          Utils.SetNull(sender as Control, "SelectedValue", partnerBindingSource.Current);
        }
      }

      StatusBar.IsError = false;
      StatusBar.Message = string.Empty;
    }
    #endregion

    #region Spremanje & brisanje
    // Spremanje svih izmjena
    protected override void DoSaveChanges()
    {
      partnerBll.SaveChanges(((PartnerList)partnerBindingSource.DataSource).GetChanges());
    }

    // Brisanje
    protected override void DoDelete()
    {
      // Uklanjanje poslovnog objekta iz liste dohva�enih objekata (ozna�ava objekt obrisanim)
      partnerBindingSource.RemoveCurrent();
      // Sprema izmjene u bazu. Objekti ozna�eni za brisanje biti �e uklonjeni iz baze.
      partnerBll.SaveChanges(((PartnerList)partnerBindingSource.DataSource).GetChanges());
    }
    #endregion

    #region Razno
    // Odabir tipa partnera
    private void tvrtkaCheck_CheckedChanged(object sender, EventArgs e)
    {
      if (InEditMode)
      {
        Partner p = partnerBindingSource.Current as Partner;
        if (p != null)
        {
          if (tvrtkaCheck.Checked)
          {
            p.TipPartnera = TipPartnera.Tvrtka;
          }
          else
          {
            p.TipPartnera = TipPartnera.Osoba;
          }
        }
      }
    }
    #endregion
  }
}