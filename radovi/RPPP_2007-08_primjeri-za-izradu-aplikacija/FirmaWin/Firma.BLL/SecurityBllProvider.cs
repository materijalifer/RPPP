using System;
using System.Collections.Generic;
using System.Text;

namespace Firma
{
  public class SecurityBllProvider
  {
    // Kako sigurnost nije tema predmeta ne�emo komplicirati. Password je plain-text...
    public bool IsAuthenticated(string username, string password)
    {
      return (new SecurityDalProvider()).IsAuthenticated(username, password);
    }
  }
}
