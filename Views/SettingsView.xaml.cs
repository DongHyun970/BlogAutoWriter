using System.Windows;
using BlogAutoWriter.Services;
using System.Windows.Controls;

namespace BlogAutoWriter.Views
{
    public partial class SettingsView : UserControl
    {
        private string grade;
        private UserSettings settings;

        public SettingsView()
        {
            InitializeComponent();
            grade = App.Current.Properties["Grade"] as string ?? "Free";
            settings = SettingsManager.Load();

            LoadSettingsToUI();
            ApplyMembershipRestrictions();
        }

        private void LoadSettingsToUI()
        {
            ApiKeyBox.Text = settings.ApiKey;
            KakaoIdBox.Text = settings.KakaoId;
            KakaoPwBox.Password = settings.KakaoPassword;
            CategoryBox.Text = settings.Category;
            TemplateComboBox.SelectedIndex = settings.Template switch
            {
                "깨끗한 템플릿" => 0,
                "모던 다크" => 1,
                "컬러풀 블록형" => 2,
                _ => 0
            };
            ImageUsageCheckBox.IsChecked = settings.UseImage;
        }

        private void SaveSettingsFromUI()
        {
            settings.ApiKey = ApiKeyBox.Text.Trim();
            settings.KakaoId = KakaoIdBox.Text.Trim();
            settings.KakaoPassword = KakaoPwBox.Password.Trim();
            settings.Category = CategoryBox.Text.Trim();
            settings.Template = (TemplateComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
            settings.UseImage = ImageUsageCheckBox.IsChecked == true;

            SettingsManager.Save(settings);
        }

        private void ApplyMembershipRestrictions()
        {
            if (grade != "VIP" && grade != "VVIP")
            {
                TemplatePanel.IsEnabled = false;
                TemplateComboBox.ToolTip = "VIP 이상 회원만 사용 가능합니다.";
            }

            if (grade != "VVIP")
            {
                ImageUsageCheckBox.IsEnabled = false;
                ImageUsageCheckBox.ToolTip = "VVIP 회원 전용 기능입니다.";
            }
        }

        // private void Close_Click(object sender, RoutedEventArgs e)
        // {
        //     SaveSettingsFromUI();
        //     this.Visibility = Visibility.Collapsed; // ✅ 변경된 부분
        // }
    }
}
