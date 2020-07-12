using System;
using System.Collections.Generic;
using System.Text;

namespace Firma
{
  // Su�elje koje implementira DAL objekt stavaka.
  // Potrebno je kako bi zaglavlje znalo u�itati svoje stavke.
  public interface IStavkaDalProvider
  {
    StavkaList FetchAll(int idDokumenta);
  }
}
