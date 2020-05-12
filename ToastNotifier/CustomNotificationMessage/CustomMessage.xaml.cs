﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ToastNotifications.Core;

namespace ToastNotifier.CustomNotificationMessage
{
    /// <summary>
    /// CustomMessage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CustomMessage : NotificationDisplayPart
    {
        public CustomMessage(CustomMessageViewModel vm)
        {
            InitializeComponent();
            Bind(vm);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }
    }
}
