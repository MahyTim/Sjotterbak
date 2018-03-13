using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sjotterbak.WebApi.Controllers;

namespace Sjotterbak.WebApi
{
    public class IndexModel : PageModel
    {
        private RankingController _controller;

        public IndexModel(RankingController controller)
        {
            _controller = controller;
        }

        [BindProperty]
        public RankingController.RankingResult Results { get; set; }

        public void OnGet()
        {
            Results = ((JsonResult) _controller.Get()).Value as RankingController.RankingResult;
        }
    }
}