using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.ComponentModel;

namespace NTier
{
  // Su�elje koje implementira svaka lista poslovnih objekata
  public interface IBusinessObjectList : IBindingList
  {
    void CancelNew(int itemIndex);
    IList GetChanges();
  }
}
