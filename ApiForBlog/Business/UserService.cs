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
    public class UserService
    {
        private ApiForBlogContext _context;

        #region [ Constructor ]
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public UserService(ApiForBlogContext context)
        {
            _context = context;
        } 
        #endregion

        #region [ GetById(int id)]
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public User GetById(int id)
        {
            return _context.Users.Where(
                u => u.Id == id).FirstOrDefault();
        }
        #endregion

        #region [ GetAll() ]
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<User> GetAll()
        {
            return _context.Users
                .OrderBy(p => p.Name).ToList();
        }
        #endregion

        #region [ Create(User user, string password) ]
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public Result Create(User user, string password)
        {
            Result result = ValidateEntity(user);
            result.Action = "Inclusão de usuário";

            // validation
            if (string.IsNullOrWhiteSpace(password))
                result.Inconsistencies.Add("Senha é obrigatório.");

            if (_context.Users.Any(x => x.Email == user.Email))
                result.Inconsistencies.Add("Email " + user.Email + " já cadastrado no sistema.");

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            if (result.Inconsistencies.Count == 0)
            {
                _context.Users.Add(user);
                _context.SaveChanges();
            }

            return result;
        }
        #endregion

        #region [ Update(User userParam, string password = null) ]
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userParam"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public Result Update(User userParam, string password = null)
        {
            Result result = ValidateEntity(userParam);
            result.Action = "Atualização de usuário";

            if (result.Inconsistencies.Count == 0)
            {
                User user = _context.Users.Where(
                    p => p.Id == userParam.Id).FirstOrDefault();

                if (user == null)
                    result.Inconsistencies.Add("Usuário não encontrado");

                if (userParam.Email != user.Email)
                {
                    if (_context.Users.Any(x => x.Email == userParam.Email))
                        result.Inconsistencies.Add("Email " + userParam.Email + " is already taken");
                }
                
                if(result.Inconsistencies.Count == 0)
                {
                    // update user properties
                    user.Name = userParam.Name;
                    user.Email = userParam.Email;

                    // update password if it was entered
                    if (!string.IsNullOrWhiteSpace(password))
                    {
                        byte[] passwordHash, passwordSalt;
                        CreatePasswordHash(password, out passwordHash, out passwordSalt);

                        user.PasswordHash = passwordHash;
                        user.PasswordSalt = passwordSalt;
                    }

                    _context.Users.Update(user);
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
            resultado.Action = "Exclusão de usuário";

            User user = GetById(id);
            if (user == null)
                resultado.Inconsistencies.Add("Usuário não encontrado");
            else
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }

            return resultado;
        }
        #endregion

        #region [ Authenticate(string email, string password) ]
        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public User Authenticate(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return null;

            var user = _context.Users.SingleOrDefault(x => x.Email == email);

            // check if username exists
            if (user == null)
                return null;

            // check if password is correct
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            // authentication successful
            return user;
        }
        #endregion

        #region [ ValidateEntity(User user) ]
        private Result ValidateEntity(User user)
        {
            var result = new Result();

            if (user == null)
                result.Inconsistencies.Add("Preencha os dados do usuário");
            else
            {
                if (String.IsNullOrWhiteSpace(user.Name))
                    result.Inconsistencies.Add("Preencha o nome do usuário");

                if (String.IsNullOrWhiteSpace(user.Email))
                    result.Inconsistencies.Add("Preencha o email do usuário");
            }

            return result;
        }
        #endregion

        #region [ CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt) ]
        /// <summary>
        /// 
        /// </summary>
        /// <param name="password"></param>
        /// <param name="passwordHash"></param>
        /// <param name="passwordSalt"></param>
        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
        #endregion

        #region [ VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt) ]
        /// <summary>
        /// 
        /// </summary>
        /// <param name="password"></param>
        /// <param name="storedHash"></param>
        /// <param name="storedSalt"></param>
        /// <returns></returns>
        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        } 
        #endregion
    }
}
