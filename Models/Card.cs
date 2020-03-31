using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;

namespace FlashCards.Models
{
    public class Card
    {
        [Key]
        public int CardId{get;set;}


        [Required(ErrorMessage = "A question is required for this card.")]
        public string Question{get;set;}


        [Required(ErrorMessage = "An answer is required for this card.")]
        public string Answer{get;set;}


        public int GroupId{get;set;}


        public Group BelongGroup{get;set;}

    /* -------------------------------------------------------------------------------- */
    // DATETIMEs
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;


    }
}