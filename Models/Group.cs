using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;

namespace FlashCards.Models
{
    public class Group
    {
        [Key]
        public int GroupId{get;set;}

        [Required(ErrorMessage="Card group needs a name.")]
        public string Name{get;set;}


        public int UserId{get;set;}

        public User User{get;set;}

        public List<Card> FlashCards{get;set;}



    /* -------------------------------------------------------------------------------- */
    // DATETIMEs
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;




    }
}