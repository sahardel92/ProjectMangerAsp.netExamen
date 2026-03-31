using ProjectManager.DAL.Models;
using ProjectManager.DAL.Repositories;

namespace ProjectManager.BLL.Services
{
    public class PostService
    {
        private readonly PostRepository _postRepository;

        public PostService(PostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public void CreatePost(Guid employeeId, Guid projectId, string subject, string content)
        {
            _postRepository.CreatePost(employeeId, projectId, subject, content);
        }

        public List<Post> GetPostsByEmployeeId(Guid employeeId)
        {
            return _postRepository.GetPostsByEmployeeId(employeeId);
        }
    }
}
