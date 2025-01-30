using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StaffandTrain.Models
{
    public class EmailTemplate
    {
        public int TemplateId { get; set; }
        public int TemplateIdDecrypt { get; set; }
        public string TemplateName { get; set; }
        public string Subject { get; set; }
        public int? GroupingNumber { get; set; }
        public string EmailBody { get; set; }
        public string EmailBodyTextArea { get; set; }
        public string IsTextorHtml { get; set; }
        public string EmailBatchVal { get; set; }
        public string BatchEmailCount { get; set; }
    }
}