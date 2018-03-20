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
    public class UserController: Controller
    {
        private UserService _service;
        private IMapper _mapper;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="service"></param>
        /// <param name="mapper"></param>
        public UserController(UserService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        /// <summary>
        /// Método para consulta de todos os usuários (Qualquer usuário)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("v1/user/getall")]
        public IActionResult GetAll()
        {
            var users = _service.GetAll();
            var userDtos = _mapper.Map<IList<UserDto>>(users);
            return Ok(userDtos);
        }

        /// <summary>
        /// Método para recuperação de um artigo (Qualquer usuário)
        /// </summary>
        /// <param name="id">identificador do usuário</param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("v1/user/getbyid")]
        public IActionResult GetById(int id)
        {
            var user = _service.GetById(id);
            if (user != null)
            {
                var userDto = _mapper.Map<UserDto>(user);
                return Ok(userDto);
            }
            else
                return NotFound();
        }

        /// <summary>
        /// Método para inserção de um usuário novo (Qualquer usuário)
        /// </summary>
        /// <param name="userDto">Objeto com as informações do usuário</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("v1/user/create")]
        public Result Create([FromBody]UserDto userDto)
        {
            // map dto to entity
            var user = _mapper.Map<User>(userDto);
            return _service.Create(user, userDto.Password);
        }

        /// <summary>
        /// Método para alteração de um usuário (Somente usuários com a role de administrador (admin))
        /// </summary>
        /// <param name="userDto">Objeto com as informações do usuário</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Policy = "AdminPolicy")]
        [Route("v1/user/update")]
        public Result Update([FromBody]UserDto userDto)
        {
            // map dto to entity and set id
            var user = _mapper.Map<User>(userDto);
          
            return _service.Update(user);
        }

        /// <summary>
        /// Método para exclusão de um usuário (Somente usuários com a role de administrador (admin))
        /// </summary>
        /// <param name="id">Objeto com as informações do usuário</param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize(Policy = "AdminPolicy")]
        [Route("v1/user/delete")]
        public Result Delete(int id)
        {
            return _service.Delete(id);
        }
    }
}
