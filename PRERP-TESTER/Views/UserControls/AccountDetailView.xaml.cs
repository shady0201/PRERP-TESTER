using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using PRERP_TESTER.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PRERP_TESTER.Views.UserControls
{
    public partial class AccountDetailView : UserControl
    {
        public ICommand TogglePermissionCommand { get; }
        public AccountDetailView()
        {
            InitializeComponent();
            TogglePermissionCommand = new RelayCommand<string?>(TogglePermissionCollapse);
        }

        public void TogglePermissionCollapse(string? section)
        {
            var account = this.DataContext as Account;
            if (account == null) return;

            switch (section)
            {
                case "SFT":
                    account.IsSftExpanded = !account.IsSftExpanded;
                    break;
                case "MASSET":
                    account.IsMassetExpanded = !account.IsMassetExpanded;
                    break;
                case "HASSET":
                    account.IsHassetExpanded = !account.IsHassetExpanded;
                    break;
            }
        }
    }
}
