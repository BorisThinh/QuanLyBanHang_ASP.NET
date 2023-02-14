using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyBanHang.Models;
using System.Net;
using System.Net.Mail;
namespace QuanLyBanHang.Controllers
{
    public class GioHangController : Controller
    {
        private qlbanhangEntities db = new qlbanhangEntities();
        // GET: GioHang
        public ActionResult Index()
        {
            List<CartItem> giohang = Session["giohang"] as List<CartItem>;
            return View(giohang);
        }
        // PHƯƠNG THỨC THÊM SẢN PHẨM VÀO GIỎ HÀNG
        public RedirectToRouteResult AddToCart(string MaSP)
        {
            if(Session["giohang"] == null)
            {
                Session["giohang"] = new List<CartItem>();
            }
                
            List<CartItem> giohang = Session["giohang"] as List<CartItem>;
            // Kiểm tra sản phẩm khách hàng chọn có trong giỏ hay chưa
            if(giohang.FirstOrDefault(m => m.MaSP == MaSP) == null) // chưa có trong giỏ hàng
            {
                SanPham sp = db.SanPhams.Find(MaSP);
                CartItem newItem = new CartItem();
                newItem.MaSP = MaSP;
                newItem.TenSP = sp.TenSP;
                newItem.SoLuong = 1;
                newItem.DonGia = Convert.ToDouble(sp.Dongia);
                giohang.Add(newItem);

            }
            else // sản phẩm đã có trong giỏ ==> tăng số lượng lên 1 
            {
                CartItem cartItem = giohang.FirstOrDefault(m => m.MaSP == MaSP);
                cartItem.SoLuong++;
            }
            Session["giohang"] = giohang;
            return RedirectToAction("Index");
        }
        public RedirectToRouteResult Update (string MaSP, int txtSoluong)
        {
            // tìm CartItem muốn sửa
            List<CartItem> giohang = Session["giohang"] as List<CartItem>;
            CartItem item = giohang.FirstOrDefault(m => m.MaSP == MaSP);
            if(item != null)
            {
                item.SoLuong = txtSoluong;
                Session["giohang"] = giohang;
            }
            return RedirectToAction("Index");
        }
        public RedirectToRouteResult DelCartItem(string MaSP)
        {
            // tìm CartItem muốn xóa
            List<CartItem> giohang = Session["giohang"] as List<CartItem>;
            CartItem item = giohang.FirstOrDefault(m => m.MaSP == MaSP);
            if(item != null)
            {
                giohang.Remove(item);
                Session["giohang"] = giohang;
            }
            return RedirectToAction("Index");
        }
        public ActionResult Order(string Email, string phone)
        {
            List<CartItem> giohang = Session["giohang"] as List<CartItem>;
            string sMsg = "<html><body><table border='1'><caption>Thông tin đặt hàng</caption>" +
                "<tr><th>STT</th><th>Tên hàng</th><th>Số lượng</th>" +
                "<th>Đơn giá</th><th>Thành tiền</th></tr>";
            int i = 0;
            double tongtien = 0;
            foreach(CartItem item in giohang)
            {
                i++;
                sMsg += "<tr>";
                sMsg += "<td>" + i.ToString() + "</td>";
                sMsg += "<td>" + item.TenSP + "</td>";
                sMsg += "<td>" + item.SoLuong.ToString() + "</td>";
                sMsg += "<td>" + item.DonGia.ToString() + "</td>";
                sMsg += "<td>" + String.Format("{0:#,###}", item.SoLuong*item.DonGia) + "</td>";
                sMsg += "<tr>";
                tongtien += item.SoLuong * item.DonGia;
            }
            sMsg += "<tr><th colspan ='5'> Tổng cộng: " + 
                String.Format("{0:#,### VNĐ}", tongtien) + "</th></tr></table>";
            MailMessage mail = new MailMessage("thinhphamvan3@gmail.com", 
                Email, "Thông tin đơn hàng", sMsg); // tên mail người gửi
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential("thinhphamvan3@gmail.com", 
                "wyxzslwuovjkrsmy"); // tên mail người gửi và mật khẩu ứng dụng
            mail.IsBodyHtml = true;
            client.Send(mail);
            return RedirectToAction("Index", "Home");
            // gửi email cho khách hàng
        }
    }
}