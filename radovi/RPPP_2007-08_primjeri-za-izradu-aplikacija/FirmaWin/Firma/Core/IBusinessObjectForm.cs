using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Firma
{
  // Su�elje preko kojega FormToolbar kontrola zna komunicirati 
  // s formom na kojoj se nalazi
  public interface IBusinessObjectForm : INotifyPropertyChanged
  {
    // Navigacija
    void First();
    void Previous();
    void Next();
    void Last();

    // Manipulacija podacima
    void New();
    void Edit();
    void Delete();
    void SaveChanges();
    void CancelChanges();

    // Svojstva koja odre�uju pona�anje i izgled toolbar-a
    bool InEditMode { get;}
    bool CanDoFirst { get;}
    bool CanDoPrevious { get;}
    bool CanDoNext { get;}
    bool CanDoLast { get;}
    bool CanDoNew { get;}
    bool CanDoEdit { get;}
    bool CanDoDelete { get;}
    bool CanDoSaveChanges { get;}
    bool CanDoCancelChanges { get;}

    // Forma odre�uje trenutak kada treba osvje�iti toolbar
    event EventHandler NeedToolbarRefresh;
  }
}
