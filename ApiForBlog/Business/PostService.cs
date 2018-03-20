using ApiForBlog.Data;
using ApiForBlog.Models;
using ApiForBlog.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApiForBlog.Business
{
    /// <summary>
    /// 
    /// </summary>
    public class PostService
    {
        private ApiForBlogContext _context;

        #region [ Constructor ]
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public PostService(ApiForBlogContext context)
        {
            _context = context;
        }
        #endregion

        #region [ GetById(int id) ]
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Post GetById(int id)
        {
            return _context.Posts.Where(
                p => p.Id == id).FirstOrDefault();

        } 
        #endregion

        #region [ GetAll() ]
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Post> GetAll()
        {
            return _context.Posts
                .OrderBy(p => p.Title).ToList();
        } 
        #endregion

        #region [ Create(Post post) ]
        /// <summary>
        /// 
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        public Result Create(Post post)
        {
            Result result = ValidateEntity(post);
            result.Action = "Inclusão de Post";

            if (result.Inconsistencies.Count == 0 &&
                _context.Posts.Where(p => p.Title == post.Title).Count() > 0)
            {
                result.Inconsistencies.Add("Post já cadastrado com este titulo");
            }

            if (result.Inconsistencies.Count == 0)
            {
                _context.Posts.Add(post);
                _context.SaveChanges();
            }

            return result;
        }
        #endregion

        #region [ Update(Post post) ]
        /// <summary>
        /// 
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        public Result Update(Post post)
        {
            Result result = ValidateEntity(post);
            result.Action = "Atualização de Post";

            if (result.Inconsistencies.Count == 0)
            {
                Post postRepository = _context.Posts.Where(
                    p => p.Id == post.Id).FirstOrDefault();

                if (postRepository == null)
                    result.Inconsistencies.Add("Post não encontrado");
                else
                {
                    postRepository.Title = post.Title;
                    postRepository.Content = post.Content;
                    _context.SaveChanges();
                }
            }

            return result;
        }
        #endregion

        #region [ Delete(int id) ]
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result Delete(int id)
        {
            Result resultado = new Result();
            resultado.Action = "Exclusão de Post";

            Post post = GetById(id);
            if (post == null)
                resultado.Inconsistencies.Add("Post não encontrado");
            else
            {
                _context.Posts.Remove(post);
                _context.SaveChanges();
            }

            return resultado;
        }
        #endregion

        #region [ ValidateEntity(Post post) ]
        /// <summary>
        /// 
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        private Result ValidateEntity(Post post)
        {
            var result = new Result();
            if (post == null)
                result.Inconsistencies.Add("Preencha os dados do post");
            else
            {
                if (String.IsNullOrWhiteSpace(post.Title))
                    result.Inconsistencies.Add("Preencha o Titulo do Post");

                if (String.IsNullOrWhiteSpace(post.Content))
                    result.Inconsistencies.Add("Preencha o conteúdo do post");
            }

            return result;
        } 
        #endregion
    }
}
