﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QL_KS.GUI
{
    public partial class frmThuePhong : Form
    {
        DAL_Phong dalPh = new DAL_Phong();
        KetNoiData cn = new KetNoiData();
        EC_PhieuThue ecPT = new EC_PhieuThue();
        EC_HoaDon ecHD = new EC_HoaDon();
        DAL_HoaDon dalHD = new DAL_HoaDon();
        private string _Gia;
        private bool _load;
        public frmThuePhong()
        {
            InitializeComponent();
        }
        public static string findCode(string name, string columnSrc, string columnDes, DataTable tb)
        {
            for (int i = 0; i < tb.Rows.Count; i++)
            {
                if (tb.Rows[i][columnSrc].ToString() == name) return tb.Rows[i][columnDes].ToString();
            }
            return null;
        }
        private void frmThuePhong_Load(object sender, EventArgs e)
        {
            _load = true;
            DateTime date = DateTime.Today.AddDays(0);
            dtpNgayvao.Value = date;
            //load dgvThongTin
            DataTable tb = dalPh.ThongTinPhong("");
            dgvThongtin.DataSource = tb;
            for (int i = 0; i < dgvThongtin.RowCount; i++)
            {
                dgvThongtin.Rows[i].Cells["STT"].Value = i + 1;
                if (dgvThongtin.Rows[i].Cells["TrangThai"].Value.ToString() == "Tốt") dgvThongtin.Rows[i].DefaultCellStyle.BackColor = Color.Lime;
                else
                    if (dgvThongtin.Rows[i].Cells["TrangThai"].Value.ToString() == "Đã thuê") dgvThongtin.Rows[i].DefaultCellStyle.BackColor = Color.LightSkyBlue;
                else
                        if (dgvThongtin.Rows[i].Cells["TrangThai"].Value.ToString() == "Hỏng") dgvThongtin.Rows[i].DefaultCellStyle.BackColor = Color.Red;
            }
            //load smbKhachhang
            tb = cn.GetDataTable("Select distinct MaKH, tenKH from tblKhachHang");
            cmbKhachhang.DataSource = tb;
            cmbKhachhang.DisplayMember = "tenKH";
            cmbKhachhang.Text = findCode(cmbKhachhang.Text, "TenKH", "MaKH", tb);

            ////load Phuongthuc
            //tb = cn.GetDataTable("Select distinct MaPT, TenPT from tblPhuongThucThue");
            //cmbPhuongthuc.DataSource = tb;
            //cmbPhuongthuc.DisplayMember = "TenPT";
            //cmbPhuongthuc.ValueMember = "MaPT";
            //cmbPhuongthuc.ResetText();
            //txtMaPT.Text = findCode(cmbPhuongthuc.Text, "TenPT", "MaPT", tb);

            //lay dl vao ec

            ecHD.NgayVao = dtpNgayvao.Value.Year.ToString() + "-" + dtpNgayvao.Value.Month.ToString() + "-" + dtpNgayvao.Value.Day.ToString();
            _load = false;
        }

        private void cmbKhachhang_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataTable tb = cn.GetDataTable("Select distinct MaKH, tenKH from tblKhachHang");
            cmbKhachhang.Text = findCode(cmbKhachhang.Text, "TenKH", "MaKH", tb);
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dgvThongtin.RowCount; i++)
            {
                dgvThongtin.Rows[i].Cells["Thue"].Value = false;
            }
        }

        private void btnThue_Click(object sender, EventArgs e)
        {
			 for(int i = 0; i<dgvThongtin.RowCount; i++)
            {
                if(Convert.ToBoolean(dgvThongtin.Rows[i].Cells["Thue"].Value)== true)
                {
                    if (dgvThongtin.Rows[i].Cells["TrangThai"].Value.ToString() == "Đã thuê")
                    {
                        MessageBox.Show("Phòng " + dgvThongtin.Rows[i].Cells["SoPhong"].Value.ToString() + " đã được thuê rồi. Chọn phòng khác!");
                        dgvThongtin.Rows[i].Cells["Thue"].Value = false;
                        return;
                    }
                    if (dgvThongtin.Rows[i].Cells["TrangThai"].Value.ToString() == "Hỏng")
                    {
                        MessageBox.Show("Phòng " + dgvThongtin.Rows[i].Cells["SoPhong"].Value.ToString() + " đang hỏng. Chọn phòng khác!");
                        dgvThongtin.Rows[i].Cells["Thue"].Value = false;
                        return;
                    }
                    ecPT.MaPh = dgvThongtin.Rows[i].Cells["MaPh"].Value.ToString();
                    ecPT.MaKH = cmbKhachhang.Text;
                    DateTime date = DateTime.Now;
                    ecPT.MaPhieu = date.Day.ToString() + date.Month.ToString() + date.Hour.ToString() + date.Minute.ToString() + date.Second.ToString() + dgvThongtin.Rows[i].Cells["SoPhong"].Value.ToString();
                    DAL_PhieuThue dalPhieuthue = new DAL_PhieuThue();
                    dalPhieuthue.ThemThongTin(ecPT);

                    ecHD.MaHD = ecPT.MaPhieu;
                    ecHD.MaPhieuThue = ecPT.MaPhieu;
                    ecHD.ThanhTien = 0;
                    ecHD.Gia = 200;
                    ecHD.NgayVao = dtpNgayvao.Value.Year.ToString()+"-"+ dtpNgayvao.Value.Month.ToString()+"-"+dtpNgayvao.Value.Day.ToString();
                    ecHD.NgayRa = dtpNgayra.Value.Year.ToString() + "-" + dtpNgayra.Value.Month.ToString() + "-" + dtpNgayra.Value.Day.ToString();
                    dalHD.ThemThongTin(ecHD);

                    EC_Phong ecPh = new EC_Phong();
                    ecPh.MaPh = dgvThongtin.Rows[i].Cells["MaPh"].Value.ToString();
                    ecPh.SoPhong = dgvThongtin.Rows[i].Cells["SoPhong"].Value.ToString();
                    ecPh.TrangThai = "Đã thuê";
                    dalPh.SuaThongTin(ecPh);
                }
            }
            MessageBox.Show("Thuê OK!");

            DataTable tb = dalPh.ThongTinPhong("");
            dgvThongtin.DataSource = tb;
            for (int i = 0; i < dgvThongtin.RowCount; i++)
            {
                dgvThongtin.Rows[i].Cells["STT"].Value = i + 1;
                if (dgvThongtin.Rows[i].Cells["TrangThai"].Value.ToString() == "Tốt") dgvThongtin.Rows[i].DefaultCellStyle.BackColor = Color.Lime;
                else
                    if (dgvThongtin.Rows[i].Cells["TrangThai"].Value.ToString() == "Đã thuê") dgvThongtin.Rows[i].DefaultCellStyle.BackColor = Color.LightSkyBlue;
                    else
                        if (dgvThongtin.Rows[i].Cells["TrangThai"].Value.ToString() == "Hỏng") dgvThongtin.Rows[i].DefaultCellStyle.BackColor = Color.Red;
            }
        }
    }
}
