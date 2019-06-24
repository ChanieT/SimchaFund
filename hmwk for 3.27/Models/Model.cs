using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hmwk_for_3._27.Models
{
    public class SimchaViewModel
    {
        public IEnumerable<Simcha> Simchas { get; set; }
        public int ContributorCount { get; set; }
        //public decimal Total { get; set; }
    }

    public class ContributorViewModel
    {
        public IEnumerable<Contributor> Contributors { get; set; }
        public decimal Total { get; set; }
    }

    public class ContributorsInViewModel
    {
        public Contributor Contributor { get; set; }
        public decimal Balance { get; set; }
    }

    public class ContributionsViewModel
    {
        public Simcha Simcha { get; set; }
        public IEnumerable<SimchaContributor> Contributors { get; set; }

    }

    public class HistoryViewModel
    {
        public Contributor Contributor { get; set; }
        public List<ContributorHistory> Actions { get; set; }
        
    }

    



}