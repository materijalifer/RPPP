using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Firma
{
  // Klasa koja mijenja zna�ku mi�a 
  // da bi se korisniku dojavilo da mora pri�ekati.
  // Koristi se...
      // using(new StatusBusy()) // postavi zna�ku na Wait
      // {
      //   ...
      // } // vrati kursor na staro
  // naredba using definira doseg nakon kojeg �e objekt biti uni�ten
  public class StatusBusy : IDisposable
  {
    private Cursor c;

    public StatusBusy()
    {
      this.c = Cursor.Current;
      Cursor.Current = Cursors.WaitCursor;
    }

    #region IDisposable Members
    void IDisposable.Dispose()
    {
      Cursor.Current = this.c;
    }
    #endregion
  }
}
