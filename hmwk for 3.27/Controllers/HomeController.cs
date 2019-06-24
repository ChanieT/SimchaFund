using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using hmwk_for_3._27.Models;

namespace hmwk_for_3._27.Controllers
{
    public class HomeController : Controller
    {
        Manager mgr = new Manager(Properties.Settings.Default.ConStr);

        public ActionResult Index()
        {
            SimchaViewModel vm = new SimchaViewModel
            {
                Simchas = mgr.GetSimchas(),
                ContributorCount = mgr.GetContributorCount()
            };
            return View(vm);
        }

        public ActionResult Contributors()
        {
            ContributorViewModel vm = new ContributorViewModel
            {
                Contributors = mgr.GetContributors(),
                Total = mgr.GetTotalBalance()
            };
            return View(vm);
        }

        public ActionResult AddSimcha()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SubmitSimcha(Simcha s)
        {
            mgr.AddSimcha(s);
            return Redirect("/home/index");
        }

        public ActionResult AddContributor()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SubmitContributor(Contributor c, decimal amount)
        {
            mgr.AddContributor(c);
            var d = new Deposit
            {
                Amount = amount,
                ContributorId = c.Id,
                Date = c.Date
            };
            mgr.SubmitDeposit(d);
            return Redirect("/home/contributors");
        }

        public ActionResult Deposit(int id)
        {
            Deposit d = new Deposit();
            d.ContributorId = id;
            return View(d);
        }
        [HttpPost]
        public ActionResult SubmitDeposit(Deposit d)
        {
            mgr.SubmitDeposit(d);
            return Redirect("/home/contributors");
        }

        public ActionResult ShowHistory(int id)
        {
            HistoryViewModel hvm = new HistoryViewModel();

            hvm.Contributor = mgr.GetContributor(id);
            IEnumerable<ContributorHistory> deposits = mgr.GetDepositHistory(id).ToList();
            IEnumerable<ContributorHistory> contributions = mgr.GetContributionsHistory(id).ToList();
            List<ContributorHistory> actions = new List<ContributorHistory>();
            foreach (ContributorHistory c in deposits)
            {
                actions.Add(c);
            }
            foreach (ContributorHistory c in contributions)
            {
                actions.Add(c);
            }
            hvm.Actions = actions;
            return View(hvm);
        }

        public ActionResult Edit(Contributor c)
        {
            mgr.EditContributor(c);
            return Redirect("/home/contributors");
        }

        public ActionResult Contributions(int id)
        {
            IEnumerable<Contributor> contributors = mgr.GetContributors();
            IEnumerable<SimchaContributor> contributed = mgr.GetContributorsThatContributed(id);
            IEnumerable<SimchaContributor> simchaContributors = contributors.Select(c => new SimchaContributor
            {
                ContributorId = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                AlwaysInclude = c.AlwaysInclude,
                Amount = mgr.GetAmount(c.Id),
                Balance = mgr.GetContributorBalance(c.Id),
                Contributed = false
            }).ToList();
            
            foreach(SimchaContributor sc in simchaContributors)
            {
                foreach(SimchaContributor s in contributed)
                {
                    if (s.ContributorId == sc.ContributorId)
                    {
                        sc.Contributed = true;
                        sc.Amount = s.Amount;
                        break;
                    }
                    
                }
            }

            ContributionsViewModel cvm = new ContributionsViewModel
            {
                Simcha = mgr.GetSimchaById(id),
                Contributors = simchaContributors
            };

            return View(cvm);
        }
        [HttpPost]
        public ActionResult UpdateContributions(IEnumerable<SimchaContributor> contributors, int simchaId)
        {
            mgr.DeleteContributions(simchaId);
            mgr.UpdateContribution(contributors, simchaId);
            return Redirect($"/home/contributions?id={simchaId}");
        }

    }
}