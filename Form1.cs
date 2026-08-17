using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OzonReturnsManager1.Models;
using OzonReturnsManager1.Services;

namespace OzonReturnsManager1
{
    public partial class Form1 : Form
    {
        private readonly TokenService _tokenService;
        private ReturnsApiClient _apiClient;
        private BindingSource _bindingSource;

        public Form1()
        {
            InitializeComponent();
            _tokenService = new TokenService();
            InitializeStatusComboBox();
            InitializeOrgTypeComboBox();
            InitializeDataGridView();
        }

        private void InitializeStatusComboBox()
        {
            cmbStatus.Items.Add("Все");
            foreach (var status in ReturnStatusExtensions.GetAllRussianStatuses())
            {
                cmbStatus.Items.Add(status);
            }
            cmbStatus.SelectedIndex = 0;
        }

        private void InitializeOrgTypeComboBox()
        {
            cmbOrgType.SelectedIndex = 0; // "Все"
        }

        private void InitializeDataGridView()
        {
            _bindingSource = new BindingSource();
            dgvReturns.DataSource = _bindingSource;
        }

        private async void btnRequest_Click(object sender, EventArgs e)
        {
            try
            {
                // Проверяем наличие токена
                if (!_tokenService.TokenExists())
                {
                    MessageBox.Show(
                        "Файл token.txt не найден. Пожалуйста, создайте файл с токеном авторизации.",
                        "Ошибка",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                var token = _tokenService.GetToken();
                _apiClient = new ReturnsApiClient(token);

                // Получаем параметры фильтрации
                var dateFrom = dtpFrom.Value.Date;
                var dateTo = dtpTo.Value.Date;
                
                string statusFilter = cmbStatus.SelectedItem?.ToString();
                if (statusFilter == "Все")
                {
                    statusFilter = null;
                }

                int? orgTypeFilter = null;
                if (cmbOrgType.SelectedIndex == 1) // Процесс-Лайн
                {
                    orgTypeFilter = 0;
                }
                else if (cmbOrgType.SelectedIndex == 2) // Время
                {
                    orgTypeFilter = 1;
                }

                btnRequest.Enabled = false;
                btnRequest.Text = "Загрузка...";
                Cursor = Cursors.WaitCursor;

                var allRecords = new List<ReturnRecord>();

                // Загружаем возвраты от покупателей
                if (string.IsNullOrEmpty(statusFilter) || statusFilter != "Завершено")
                {
                    var customerReturns = await _apiClient.GetCustomerReturnsAsync(
                        dateFrom, dateTo, orgTypeFilter, statusFilter);
                    allRecords.AddRange(customerReturns);
                }

                // Загружаем вывоз со склада
                var stockRemovals = await _apiClient.GetStockRemovalsAsync(
                    dateFrom, dateTo, orgTypeFilter, statusFilter ?? "НОВЫЙ");
                allRecords.AddRange(stockRemovals);

                // Сортируем по дате (новые сверху)
                allRecords = allRecords.OrderByDescending(r => r.Date).ToList();

                _bindingSource.DataSource = allRecords;
                dgvReturns.AutoResizeColumns();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка при загрузке данных: {ex.Message}",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                btnRequest.Enabled = true;
                btnRequest.Text = "Запросить";
                Cursor = Cursors.Default;
            }
        }
    }
}
