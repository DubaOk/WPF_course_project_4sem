using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Coach_search.ViewModels;
using System.Windows.Controls;

namespace Coach_search.MVVM.View
{
    public partial class EnrollPage : Page
    {
        public EnrollPage(int clientId, int tutorId, string tutorName)
        {
            InitializeComponent();
            DataContext = new EnrollViewModel(clientId, tutorId, tutorName);
        }
    }
}