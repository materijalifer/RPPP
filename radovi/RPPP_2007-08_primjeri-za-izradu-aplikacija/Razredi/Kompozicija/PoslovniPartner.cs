using System;
using System.Collections.Generic;
using System.Text;


class PoslovniPartner
{
  private string maticniBroj;
  private string adresaSjedista;
  private string adresaIsporuke;

  public PoslovniPartner(string MaticniBroj, string AdresaSjedista, string AdresaIsporuke)
  {
    this.maticniBroj = MaticniBroj;
    this.adresaSjedista = AdresaSjedista;
    this.adresaIsporuke = AdresaIsporuke;
  }

  ~PoslovniPartner()  
  {
    Console.Write("~Partner");
  }

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


  //Override metode ToString() koja je naslje�ena iz razreda System.Object
  public override string ToString() //Ovu metodu nije potrebno implementirati u razredu koji naslje�uje ovaj! Ukoliko je implementriamo potrebno je dodati klju�nu rije� override!
  {
    return maticniBroj +
      "\nAdresa Sjedi�ta: \n" + adresaSjedista +
      "\nAdresa Isporuke: \n" + adresaIsporuke;
  }

}
