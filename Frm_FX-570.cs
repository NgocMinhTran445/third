using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using System.Text.RegularExpressions;

namespace FX_570
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            txt_hienThi.Enabled = false;
            kqua.Enabled = false;
        }

        string bieuThuc = "";
        private bool giaTri = false;

        // Khi bấm nút số hoặc dấu "."
        private void btnSo_click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string kyTu = btn.Text;

            // Nếu vừa bấm "=" (giaTri == true) hoặc đang hiển thị "0", bắt đầu chuỗi mới
            if (giaTri || txt_hienThi.Text == "0")
            {
                // Nếu bấm ".", muốn hiện "0."
                if (kyTu == ".")
                {
                    txt_hienThi.Text = "0.";
                    bieuThuc = "0.";
                }
                else
                {
                    txt_hienThi.Text = kyTu;
                    bieuThuc = kyTu;
                }
                giaTri = false;
            }
            else
            {
                // tránh thêm nhiều "." không hợp lệ
                if (kyTu == "." && txt_hienThi.Text.Contains("."))
                    return;

                txt_hienThi.Text += kyTu;
                bieuThuc += kyTu;
            }
        }

        // Khi bấm nút phép toán (+, -, x, /, ^)
        private void btnPhepTinhToan_click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string pheptinh = btn.Text;

            // Chuyển ký hiệu nhân chia về đúng dạng DataTable hiểu
            if (pheptinh == "x") pheptinh = "*";
            if (pheptinh == "÷") pheptinh = "/";

            bieuThuc += pheptinh;
            txt_hienThi.Text = bieuThuc;
        }

        // Khi bấm các hàm sin, cos, tan, sqrt
        private void btnHam_click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string ham = btn.Text;

            // Thêm cú pháp như sin( ... )
            bieuThuc += ham + "(";
            txt_hienThi.Text = bieuThuc;
        }

        // Khi bấm nút ngoặc
        private void btnNgoac_click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            bieuThuc += btn.Text;
            txt_hienThi.Text = bieuThuc;
        }

        // Khi bấm nút AC (Clear)
        private void btn_off_Click(object sender, EventArgs e)
        {
            bieuThuc = "";
            txt_hienThi.Text = "\0";
            kqua.Text = "\0";
        }

        // Khi bấm nút =
        private void btn_bang_Click(object sender, EventArgs e)
        {
            try
            {
                double kq = TinhBieuThuc(bieuThuc);
                kqua.Text = kq.ToString(CultureInfo.InvariantCulture);
                bieuThuc = kq.ToString(CultureInfo.InvariantCulture);
            }
            catch
            {
                kqua.Text = "Lỗi cú pháp";
                bieuThuc = "";
            }
        }

        // Hàm tính toán biểu thức (hỗ trợ sin, cos, tan, sqrt, ^)
        private double TinhBieuThuc(string expr)
        {
            expr = expr.Replace("√", "sqrt");

            // Xử lý lũy thừa "^" bằng Math.Pow(a, b)
            while (expr.Contains("^"))
            {
                expr = Regex.Replace(expr,
                    @"(\d+(\.\d+)?|\([^()]+\))\^(\d+(\.\d+)?|\([^()]+\))",
                    new MatchEvaluator(match =>
                    {
                        string[] parts = match.Value.Split('^');
                        double a = TinhBieuThuc(parts[0]);
                        double b = TinhBieuThuc(parts[1]);
                        return Math.Pow(a, b).ToString(CultureInfo.InvariantCulture);
                    }));
            }

            // Thay các hàm toán học bằng cú pháp C# hợp lệ
            expr = expr.Replace("sin", "Math.Sin")
                       .Replace("cos", "Math.Cos")
                       .Replace("tan", "Math.Tan")
                       .Replace("sqrt", "Math.Sqrt");

            // Dùng DataTable để tính phần còn lại
            var result = new DataTable().Compute(expr, null);
            return double.Parse(result.ToString(), CultureInfo.InvariantCulture);
        }

        private void btn_ngoacTrong_Click(object sender, EventArgs e)
        {
            txt_hienThi.Text += ")";
        }

        private void btn_del_Click(object sender, EventArgs e)
        {
            // Nếu đang ở trạng thái vừa tính xong (giaTri == true), DEL nên xóa cả kết quả
            if (giaTri)
            {
                bieuThuc = "";
                txt_hienThi.Text = "0";
                giaTri = false;
                return;
            }

            // Nếu bieuThuc có ký tự thì xóa, đồng thời cập nhật txt_hienThi
            if (!string.IsNullOrEmpty(bieuThuc))
            {
                bieuThuc = bieuThuc.Substring(0, bieuThuc.Length - 1);
                if (string.IsNullOrEmpty(bieuThuc))
                    txt_hienThi.Text = "0";
                else
                    txt_hienThi.Text = bieuThuc;
            }
            else
            {
                // Dự phòng: nếu bieuThuc rỗng nhưng txt_hienThi ko rỗng (đôi khi có), xóa từ txt_hienThi
                if (!string.IsNullOrEmpty(txt_hienThi.Text) && txt_hienThi.Text.Length > 0)
                {
                    txt_hienThi.Text = txt_hienThi.Text.Substring(0, txt_hienThi.Text.Length - 1);
                    if (string.IsNullOrEmpty(txt_hienThi.Text))
                        txt_hienThi.Text = "0";
                    bieuThuc = txt_hienThi.Text == "0" ? "" : txt_hienThi.Text;
                }
            }
        }

        private void btn_ngoacNgoai_Click(object sender, EventArgs e)
        {
            txt_hienThi.Text += "(";
        }
    }
}
