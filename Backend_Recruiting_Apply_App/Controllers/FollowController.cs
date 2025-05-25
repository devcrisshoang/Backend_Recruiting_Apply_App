using Microsoft.AspNetCore.Mvc;
using Backend_Recruiting_Apply_App.Data.Entities;
using Backend_Recruiting_Apply_App.Services;

namespace Backend_Recruiting_Apply_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FollowController : ControllerBase
    {
        private readonly IFollowService _followService;

        public FollowController(IFollowService followService)
        {
            _followService = followService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Follow>>> GetFollow()
        {
            var follows = await _followService.GetAllFollowsAsync();
            return Ok(follows);
        }

        [HttpGet("{applicantId}/{companyId}")]
        public async Task<ActionResult<Follow>> GetFollowByApplicantId(int applicantId, int companyId)
        {
            var follow = await _followService.GetFollowByApplicantAndCompanyAsync(applicantId, companyId);
            return Ok(follow);
        }

        [HttpGet("applicant/{id}")]
        public async Task<ActionResult<IEnumerable<Follow>>> GetFollowedCompanyByApplicantId(int id)
        {
            var follows = await _followService.GetFollowedCompaniesByApplicantIdAsync(id);
            return Ok(follows);
        }

        [HttpGet("companyList/applicant/{id}")]
        public async Task<ActionResult<IEnumerable<Company>>> GetFollowedCompanyListByApplicantId(int id)
        {
            var companies = await _followService.GetFollowedCompanyListByApplicantIdAsync(id);
            return Ok(companies);
        }

        [HttpPost]
        public async Task<ActionResult<Follow>> CreateFollow(Follow follow)
        {
            var createdFollow = await _followService.CreateFollowAsync(follow);
            return Ok(createdFollow);
        }

        [HttpDelete("{applicantId}/{companyId}")]
        public async Task<ActionResult<Follow>> DeleteFollow(int applicantId, int companyId)
        {
            var success = await _followService.DeleteFollowAsync(applicantId, companyId);
            return Ok();
        }
    }
}