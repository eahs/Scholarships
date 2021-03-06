﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Scholarships.Models
{
    public class Article
    {
        [Key]
        public int ArticleId { get; set; }

        [Required]
        public string Title { get; set; }
        public string Author { get; set; }
        [DisplayName("Frontpage Lead Paragraph")]
        public string Lead { get; set; }
        public string Body { get; set; }
        public DateTime PublishDate { get; set; }
        public bool Published { get; set; } = false;

    }
}
