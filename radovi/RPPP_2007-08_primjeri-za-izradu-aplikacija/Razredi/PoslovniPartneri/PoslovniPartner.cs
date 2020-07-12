using System;
using System.Collections.Generic;
using System.Text;


abstract class PoslovniPartner
{
  private string maticniBroj;
  private string adresaSjedista;
  private string adresaIsporuke;

  public string MaticniBroj
  {
    get { return maticniBroj; }
  }
  public string AdresaSjedista
  {
    set { adresaSjedista = value; }
    get { return adresaSjedista; }
  }
  public string AdresaIsporuke
  {
    set { adresaIsporuke = value; }
    get { return adresaIsporuke; }
  }

  public PoslovniPartner(string maticniBroj, string adresaSjedista, string adresaIsporuke)
  {
    this.maticniBroj = maticniBroj;
    if (!this.ValidacijaMaticnogBroja()) throw new Exception("Pogre�ka unosa mati�nog broja!");
    this.adresaSjedista = adresaSjedista;
    this.adresaIsporuke = adresaIsporuke;
  }

  //Override metode ToString() koja je naslje�ena iz razreda System.Object
  public override string ToString() //Ovu metodu nije potrebno implementirati u razredu koji naslje�uje ovaj! Ukoliko je implementriamo potrebno je dodati klju�nu rije� override!
  {
    return maticniBroj +
      "\nAdresa Sjedi�ta: \n" + adresaSjedista +
      "\nAdresa Isporuke: \n" + adresaIsporuke;
  }

  public abstract bool ValidacijaMaticnogBroja(); //Zbog abstract, ovu metodu potrebno je implementirati u razredu koji naslje�uje ovaj! Tako�er, abstract zna�i da je ne�emo implementirati ovdje!

}


