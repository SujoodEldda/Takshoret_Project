using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CofeeShop.Models
{
    public class Image
    {
        public HttpPostedFileBase image { set; get; }
    }
}