using System;
using System.Collections.Generic;
using System.Text;

namespace NTier
{
  // Stanja u kojima mo�e biti poslovni objekt
  public enum BusinessObjectState
  {
    Unmodified,
    New,
    Modified,
    Deleted
  }
}
