namespace ApiForBlog.Models
{
    public class Post
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public User User { get; set; }
    }
}
