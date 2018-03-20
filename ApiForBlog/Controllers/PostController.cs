using ApiForBlog.Business;
using ApiForBlog.Dto;
using ApiForBlog.Models;
using ApiForBlog.Util;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace ApiForBlog.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Produces("application/json")]
    public class PostController: Controller
    {
        private PostService _service;
        private IMapper _mapper;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="service"></param>
        /// <param name="mapper"></param>
        public PostController(PostService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        /// <summary>
        /// Método para consulta de todos os posts (Somente usuários com a role de usuário (user))
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Policy = "UserPolicy")]
        [Route("v1/post/getall")]
        public IActionResult GetAll()
        {
            var posts = _service.GetAll();
            var postDtos = _mapper.Map<IList<PostDto>>(posts);
            return Ok(postDtos);
        }

        /// <summary>
        /// Método para recuperação de um artigo (Qualquer usuário)
        /// </summary>
        /// <param name="id">identificador do artigo</param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("v1/post/getbyid")]
        public IActionResult GetById(int id)
        {
            var post = _service.GetById(id);
            if (post != null)
            {
                var postDto = _mapper.Map<PostDto>(post);
                return Ok(postDto);
            }
            else
                return NotFound();
        }

        /// <summary>
        /// Método para inserção de um artigo novo (Somente usuários com a role de administrador (admin))
        /// </summary>
        /// <param name="postDto">Objeto com as informações do artigo</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Policy = "AdminPolicy")]
        [Route("v1/post/create")]
        public Result Create([FromBody]PostDto postDto)
        {
            var post = _mapper.Map<Post>(postDto);
            return _service.Create(post);
        }

        /// <summary>
        /// Método para alteração de um artigo (Somente usuários com a role de administrador (admin))
        /// </summary>
        /// <param name="postDto">Objeto com as informações do artigo</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Policy = "AdminPolicy")]
        [Route("v1/post/update")]
        public Result Update([FromBody]PostDto postDto)
        {
            // map dto to entity and set id
            var post = _mapper.Map<Post>(postDto);
            return _service.Update(post);
        }

        /// <summary>
        /// Método para exclusão de um artigo (Somente usuários com a role de administrador (admin))
        /// </summary>
        /// <param name="id">identificador do artigo</param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize(Policy = "AdminPolicy")]
        [Route("v1/post/delete")]
        public Result Delete(int id)
        {
            return _service.Delete(id);
        }
    }
}
