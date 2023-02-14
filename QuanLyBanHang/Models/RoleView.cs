using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace QuanLyBanHang.Models
{
    public class RoleView
    {
        public int Id { get; set; }
        [Display(Name="Role Name")]
        public string Name { get; set; }



    }
}