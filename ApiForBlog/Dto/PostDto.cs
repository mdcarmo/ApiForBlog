namespace ApiForBlog.Dto
{
    public class PostDto
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}
