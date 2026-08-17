using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OzonReturnsApp
{
    // Перечисление статусов
    public enum EOurReturnStatus
    {
        ALL = 0, // Специальный статус для фильтра "Все"
        NEW = 1,
        ACCEPTED = 2,
        DISPUTE = 3,
        C1ACCEPTED = 4,
        WRITTENOFF = 5
    }

    public static class StatusHelper
    {
        public static string GetStatusString(EOurReturnStatus status)
        {
            switch (status)
            {
                case EOurReturnStatus.NEW: return "НОВЫЙ";
                case EOurReturnStatus.ACCEPTED: return "ОПРИХОДОВАН";
                case EOurReturnStatus.DISPUTE: return "ОТКРЫТ СПОР";
                case EOurReturnStatus.C1ACCEPTED: return "1c ОПРИХОДОВАН";
                case EOurReturnStatus.WRITTENOFF: return "СПИСАН";
                default: return "";
            }
        }

        public static EOurReturnStatus GetStatusFromString(string s)
        {
            if (string.IsNullOrEmpty(s)) return EOurReturnStatus.ALL;
            switch (s)
            {
                case "НОВЫЙ": return EOurReturnStatus.NEW;
                case "ОПРИХОДОВАН": return EOurReturnStatus.ACCEPTED;
                case "ОТКРЫТ СПОР": return EOurReturnStatus.DISPUTE;
                case "1c ОПРИХОДОВАН": return EOurReturnStatus.C1ACCEPTED;
                case "СПИСАН": return EOurReturnStatus.WRITTENOFF;
                default: return EOurReturnStatus.ALL;
            }
        }
    }

    // Базовый класс для отображения в таблице
    public class ReturnItem
    {
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; } // "Возврат" или "Вывоз"
        public string ShopName { get; set; }
        public string OfferId { get; set; }

        // Свойство только для привязки к DataGridView, чтобы отображать дату красиво
        public string DateStr => Date.ToString("dd.MM.yyyy HH:mm");
    }

    // Модели для "Возвраты от покупателей"
    // ВАЖНО: Все ID и SKU изменены на long
    public class ReturnsResponse
    {
        public string status { get; set; }
        public List<ReturnRecord> items { get; set; }
    }

    public class ReturnRecord
    {
        public long id { get; set; }
        public string shop_name { get; set; }
        public long ozon_return_id { get; set; }
        public string type { get; set; }
        public long sku { get; set; } // Было int, стало long
        public string offer_id { get; set; }
        public string name { get; set; }
        public int quantity { get; set; }
        public string posting_number { get; set; }
        public string our_status { get; set; }
        public int org_type { get; set; }
        public string change_moment { get; set; }
        public JsonDetails json { get; set; }
    }

    public class JsonDetails
    {
        public Visual visual { get; set; }
    }

    public class Visual
    {
        public string change_moment { get; set; }
    }

    // Модели для "Вывоз со склада Озон"
    // ВАЖНО: Все ID изменены на long
    public class StockResponse
    {
        public string status { get; set; }
        public List<StockRecord> data { get; set; }
    }

    public class StockRecord
    {
        public long id { get; set; }
        public string offer_id { get; set; }
        public string shop_name { get; set; }
        public int org_type { get; set; }
        public string our_status { get; set; }
        public int quantity_for_return { get; set; }
        public string box_id { get; set; }
        public string return_id { get; set; }
        public string stock_type { get; set; }
        public string return_created_at { get; set; }
        public string return_completion_at { get; set; }
        public RawData raw { get; set; }
    }

    public class RawData
    {
        public long sku { get; set; } // Было int, стало long
        public string name { get; set; }
        public string box_id { get; set; }
        public string offer_id { get; set; }
    }

    public partial class MainForm : Form
    {
        private readonly string _tokenFilePath;
        private string _token;
        private readonly HttpClient _httpClient;
        private BindingSource _bindingSource;

        public MainForm()
        {
            InitializeComponent();

            // Путь к файлу токена: рядом с exe или в папке проекта при отладке
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            // Если запускаем из bin/Debug, файл может лежать там же, 
            // но при разработке удобно брать из папки выше (root проекта)
            // Попробуем найти файл token.txt
            _tokenFilePath = Path.Combine(baseDir, "token.txt");

            if (!File.Exists(_tokenFilePath))
            {
                // Пробуем найти в родительской директории (для удобства разработки)
                var parentDir = Directory.GetParent(baseDir)?.Parent?.FullName;
                if (parentDir != null)
                {
                    _tokenFilePath = Path.Combine(parentDir, "token.txt");
                }
            }

            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(30);

            SetupGrid();
            LoadToken();
            InitializeFilters();
        }

        private void SetupGrid()
        {
            _bindingSource = new BindingSource();
            dataGridView1.DataSource = _bindingSource;

            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.Columns.Clear();

            // Колонка ID
            var colId = new DataGridViewTextBoxColumn();
            colId.Name = "colId";
            colId.HeaderText = "ID";
            colId.DataPropertyName = "Id";
            colId.Width = 80;
            dataGridView1.Columns.Add(colId);

            // Колонка Дата
            var colDate = new DataGridViewTextBoxColumn();
            colDate.Name = "colDate";
            colDate.HeaderText = "Дата";
            colDate.DataPropertyName = "DateStr";
            colDate.Width = 150;
            dataGridView1.Columns.Add(colDate);

            // Колонка Тип
            var colType = new DataGridViewTextBoxColumn();
            colType.Name = "colType";
            colType.HeaderText = "Тип";
            colType.DataPropertyName = "Type";
            colType.Width = 100;
            dataGridView1.Columns.Add(colType);

            // Колонка Магазин
            var colShop = new DataGridViewTextBoxColumn();
            colShop.Name = "colShop";
            colShop.HeaderText = "Магазин";
            colShop.DataPropertyName = "ShopName";
            colShop.Width = 150;
            dataGridView1.Columns.Add(colShop);

            // Колонка Offer ID
            var colOffer = new DataGridViewTextBoxColumn();
            colOffer.Name = "colOffer";
            colOffer.HeaderText = "Offer ID";
            colOffer.DataPropertyName = "OfferId";
            colOffer.Width = 150;
            dataGridView1.Columns.Add(colOffer);

            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
        }

        private void InitializeFilters()
        {
            // Заполняем комбобокс статусов
            comboBoxStatus.Items.Clear();
            comboBoxStatus.Items.Add("Все");
            comboBoxStatus.Items.Add("НОВЫЙ");
            comboBoxStatus.Items.Add("ОПРИХОДОВАН");
            comboBoxStatus.Items.Add("ОТКРЫТ СПОР");
            comboBoxStatus.Items.Add("1c ОПРИХОДОВАН");
            comboBoxStatus.Items.Add("СПИСАН");
            comboBoxStatus.SelectedIndex = 0; // По умолчанию "Все"

            // Заполняем комбобокс Org Type
            comboBoxOrgType.Items.Clear();
            comboBoxOrgType.Items.Add("Все");
            comboBoxOrgType.Items.Add("Процесс-Лайн (0)");
            comboBoxOrgType.Items.Add("Время (1)");
            comboBoxOrgType.SelectedIndex = 0;
        }

        private void LoadToken()
        {
            if (File.Exists(_tokenFilePath))
            {
                try
                {
                    _token = File.ReadAllText(_tokenFilePath).Trim();
                    labelTokenStatus.Text = "Токен загружен";
                    labelTokenStatus.ForeColor = System.Drawing.Color.Green;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка чтения токена: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    labelTokenStatus.Text = "Ошибка токена";
                    labelTokenStatus.ForeColor = System.Drawing.Color.Red;
                }
            }
            else
            {
                MessageBox.Show($"Файл token.txt не найден по пути: {_tokenFilePath}\n\nСоздайте файл и поместите туда токен.", "Файл токена не найден", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                labelTokenStatus.Text = "Токен не найден";
                labelTokenStatus.ForeColor = System.Drawing.Color.Red;
                _token = "";
            }
        }

        private async void btnRequest_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_token))
            {
                MessageBox.Show("Токен не загружен. Проверьте наличие файла token.txt.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnRequest.Enabled = false;
            labelStatus.Text = "Загрузка...";
            _bindingSource.DataSource = null;
            dataGridView1.Rows.Clear();

            try
            {
                var dateFrom = dateTimePickerFrom.Value.ToString("yyyy-MM-dd");
                var dateTo = dateTimePickerTo.Value.ToString("yyyy-MM-dd");

                string statusFilter = comboBoxStatus.SelectedItem.ToString();
                if (statusFilter == "Все") statusFilter = "";

                string orgTypeFilter = comboBoxOrgType.SelectedItem.ToString();
                int? orgTypeValue = null;
                if (orgTypeFilter.Contains("(0)")) orgTypeValue = 0;
                if (orgTypeFilter.Contains("(1)")) orgTypeValue = 1;

                var allItems = new List<ReturnItem>();

                // 1. Запрос "Возвраты от покупателей"
                // Фильтр по статусу применяем только если выбран конкретный статус, иначе не передаем или передаем все
                // API ожидает русское название статуса, если нужно фильтровать
                var returnsData = await FetchReturnsAsync(dateFrom, dateTo, orgTypeValue, statusFilter);
                if (returnsData != null)
                {
                    foreach (var item in returnsData)
                    {
                        // Дополнительная фильтрация на клиенте, если API вернуло лишнее
                        if (!string.IsNullOrEmpty(statusFilter) && item.our_status != statusFilter) continue;
                        if (orgTypeValue.HasValue && item.org_type != orgTypeValue.Value) continue;

                        DateTime dt;
                        if (!string.IsNullOrEmpty(item.change_moment))
                            DateTime.TryParse(item.change_moment, out dt);
                        else dt = DateTime.MinValue;

                        allItems.Add(new ReturnItem
                        {
                            Id = item.id,
                            Date = dt,
                            Type = "Возврат",
                            ShopName = item.shop_name,
                            OfferId = item.offer_id
                        });
                    }
                }

                // 2. Запрос "Вывоз со склада Озон"
                // Для вывоза логика статусов может отличаться, но в ТЗ сказано использовать тот же фильтр our_status
                // Пример запроса использовал "return_state": "Завершено", но в ТЗ фильтр по our_status
                var stockData = await FetchStockAsync(dateFrom, dateTo, orgTypeValue, statusFilter);
                if (stockData != null)
                {
                    foreach (var item in stockData)
                    {
                        if (!string.IsNullOrEmpty(statusFilter) && item.our_status != statusFilter) continue;
                        if (orgTypeValue.HasValue && item.org_type != orgTypeValue.Value) continue;

                        DateTime dt;
                        if (!string.IsNullOrEmpty(item.return_completion_at))
                            DateTime.TryParse(item.return_completion_at, out dt);
                        else dt = DateTime.MinValue;

                        allItems.Add(new ReturnItem
                        {
                            Id = item.id,
                            Date = dt,
                            Type = "Вывоз",
                            ShopName = item.shop_name,
                            OfferId = item.offer_id
                        });
                    }
                }

                _bindingSource.DataSource = allItems;
                labelStatus.Text = $"Найдено записей: {allItems.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                labelStatus.Text = "Ошибка";
            }
            finally
            {
                btnRequest.Enabled = true;
            }
        }

        private async Task<List<ReturnRecord>> FetchReturnsAsync(string from, string to, int? orgType, string status)
        {
            var url = "http://95.84.154.33:7780/api/arm/returns/getV2";

            var payload = new
            {
                changeMomentFrom = from,
                changeMomentTo = to,
                org_type = orgType, // Может быть null, JSON сериализатор это обработает
                our_status = string.IsNullOrEmpty(status) ? null : status
            };

            // Убираем null поля для чистоты запроса, если они не нужны серверу при отсутствии фильтра
            var args = new JObject(
                new JProperty("changeMomentFrom", from),
                new JProperty("changeMomentTo", to)
            );
            if (orgType.HasValue) args["org_type"] = orgType.Value;
            if (!string.IsNullOrEmpty(status)) args["our_status"] = status;

            var content = new StringContent(args.ToString(), Encoding.UTF8, "application/json");

            // Исправление заголовка Authorization для .NET Framework
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", _token);

            var response = await _httpClient.PostAsync(url, content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Ошибка HTTP (Returns): {response.StatusCode} - {responseString}");
            }

            var result = JsonConvert.DeserializeObject<ReturnsResponse>(responseString);
            if (result != null && result.status == "ok")
            {
                return result.items;
            }
            return new List<ReturnRecord>();
        }

        private async Task<List<StockRecord>> FetchStockAsync(string from, string to, int? orgType, string status)
        {
            var url = "http://95.84.154.33:7780/api/arm/from_stock/getRecords";

            var args = new JObject(
                new JProperty("return_completion_at", new JObject(
                    new JProperty("from", from),
                    new JProperty("to", to)
                ))
            );

            // В примере запроса был return_state: "Завершено". 
            // Если нужно всегда запрашивать только завершенные, раскомментируйте строку ниже:
            // args["return_state"] = "Завершено";

            if (orgType.HasValue) args["org_type"] = orgType.Value; // Проверьте, ждет ли этот метод org_type напрямую или внутри объекта
            if (!string.IsNullOrEmpty(status)) args["our_status"] = status;

            var content = new StringContent(args.ToString(), Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", _token);

            var response = await _httpClient.PostAsync(url, content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Ошибка HTTP (Stock): {response.StatusCode} - {responseString}");
            }

            var result = JsonConvert.DeserializeObject<StockResponse>(responseString);
            if (result != null && result.status == "ok")
            {
                return result.data;
            }
            return new List<StockRecord>();
        }
    }

    // Класс формы, созданный дизайнером (упрощенная версия для Program.cs)
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private Button btnRequest;
        private DataGridView dataGridView1;
        private DateTimePicker dateTimePickerFrom;
        private DateTimePicker dateTimePickerTo;
        private Label labelFrom;
        private Label labelTo;
        private Label labelStatus;
        private ComboBox comboBoxStatus;
        private Label labelStatusFilter;
        private ComboBox comboBoxOrgType;
        private Label labelOrgFilter;
        private Label labelTokenStatus;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.btnRequest = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.dateTimePickerFrom = new System.Windows.Forms.DateTimePicker();
            this.dateTimePickerTo = new System.Windows.Forms.DateTimePicker();
            this.labelFrom = new System.Windows.Forms.Label();
            this.labelTo = new System.Windows.Forms.Label();
            this.labelStatus = new System.Windows.Forms.Label();
            this.comboBoxStatus = new System.Windows.Forms.ComboBox();
            this.labelStatusFilter = new System.Windows.Forms.Label();
            this.comboBoxOrgType = new System.Windows.Forms.ComboBox();
            this.labelOrgFilter = new System.Windows.Forms.Label();
            this.labelTokenStatus = new System.Windows.Forms.Label();

            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // labelTokenStatus
            // 
            this.labelTokenStatus.AutoSize = true;
            this.labelTokenStatus.Location = new System.Drawing.Point(12, 9);
            this.labelTokenStatus.Name = "labelTokenStatus";
            this.labelTokenStatus.Size = new System.Drawing.Size(80, 13);
            this.labelTokenStatus.TabIndex = 10;
            this.labelTokenStatus.Text = "Статус токена";
            // 
            // labelFrom
            // 
            this.labelFrom.AutoSize = true;
            this.labelFrom.Location = new System.Drawing.Point(12, 35);
            this.labelFrom.Name = "labelFrom";
            this.labelFrom.Size = new System.Drawing.Size(40, 13);
            this.labelFrom.TabIndex = 1;
            this.labelFrom.Text = "С даты:";
            // 
            // dateTimePickerFrom
            // 
            this.dateTimePickerFrom.Location = new System.Drawing.Point(15, 51);
            this.dateTimePickerFrom.Name = "dateTimePickerFrom";
            this.dateTimePickerFrom.Size = new System.Drawing.Size(150, 20);
            this.dateTimePickerFrom.TabIndex = 2;
            // 
            // labelTo
            // 
            this.labelTo.AutoSize = true;
            this.labelTo.Location = new System.Drawing.Point(180, 35);
            this.labelTo.Name = "labelTo";
            this.labelTo.Size = new System.Drawing.Size(40, 13);
            this.labelTo.TabIndex = 3;
            this.labelTo.Text = "По дату:";
            // 
            // dateTimePickerTo
            // 
            this.dateTimePickerTo.Location = new System.Drawing.Point(183, 51);
            this.dateTimePickerTo.Name = "dateTimePickerTo";
            this.dateTimePickerTo.Size = new System.Drawing.Size(150, 20);
            this.dateTimePickerTo.TabIndex = 4;
            // 
            // labelStatusFilter
            // 
            this.labelStatusFilter.AutoSize = true;
            this.labelStatusFilter.Location = new System.Drawing.Point(350, 35);
            this.labelStatusFilter.Name = "labelStatusFilter";
            this.labelStatusFilter.Size = new System.Drawing.Size(42, 13);
            this.labelStatusFilter.TabIndex = 5;
            this.labelStatusFilter.Text = "Статус:";
            // 
            // comboBoxStatus
            // 
            this.comboBoxStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxStatus.Location = new System.Drawing.Point(353, 51);
            this.comboBoxStatus.Name = "comboBoxStatus";
            this.comboBoxStatus.Size = new System.Drawing.Size(150, 21);
            this.comboBoxStatus.TabIndex = 6;
            // 
            // labelOrgFilter
            // 
            this.labelOrgFilter.AutoSize = true;
            this.labelOrgFilter.Location = new System.Drawing.Point(520, 35);
            this.labelOrgFilter.Name = "labelOrgFilter";
            this.labelOrgFilter.Size = new System.Drawing.Size(66, 13);
            this.labelOrgFilter.TabIndex = 7;
            this.labelOrgFilter.Text = "Организация:";
            // 
            // comboBoxOrgType
            // 
            this.comboBoxOrgType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxOrgType.Location = new System.Drawing.Point(523, 51);
            this.comboBoxOrgType.Name = "comboBoxOrgType";
            this.comboBoxOrgType.Size = new System.Drawing.Size(150, 21);
            this.comboBoxOrgType.TabIndex = 8;
            // 
            // btnRequest
            // 
            this.btnRequest.Location = new System.Drawing.Point(690, 49);
            this.btnRequest.Name = "btnRequest";
            this.btnRequest.Size = new System.Drawing.Size(100, 25);
            this.btnRequest.TabIndex = 9;
            this.btnRequest.Text = "Запросить";
            this.btnRequest.UseVisualStyleBackColor = true;
            this.btnRequest.Click += new System.EventHandler(this.btnRequest_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(15, 90);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(775, 400);
            this.dataGridView1.TabIndex = 0;
            // 
            // labelStatus
            // 
            this.labelStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(12, 500);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(0, 13);
            this.labelStatus.TabIndex = 11;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 520);
            this.Controls.Add(this.labelTokenStatus);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.btnRequest);
            this.Controls.Add(this.comboBoxOrgType);
            this.Controls.Add(this.labelOrgFilter);
            this.Controls.Add(this.comboBoxStatus);
            this.Controls.Add(this.labelStatusFilter);
            this.Controls.Add(this.dateTimePickerTo);
            this.Controls.Add(this.labelTo);
            this.Controls.Add(this.dateTimePickerFrom);
            this.Controls.Add(this.labelFrom);
            this.Controls.Add(this.dataGridView1);
            this.Name = "MainForm";
            this.Text = "Ozon Returns & Stock";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }

    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}