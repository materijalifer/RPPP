using System;
using System.Collections.Generic;
using System.Text;

namespace NTier
{
  // Delegat kojim poslovni objekt tra�i BLL objekt u svrhu validacije
  public delegate IBllObject NeedBllObjectEventHandler();
}
