using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Calabonga.OperationResults;
using WebApplication1.Models;

namespace WebApplication1
{
    /// <summary>
    /// Custom account manager
    /// </summary>
    public interface IAccountMananger {
        Task<OperationResult<ValidatedUser>> AuthorizeUserAsync(LoginViewModel data);
    }

    public class AccountMananger : IAccountMananger {
        /// <summary>
        /// Return true if user exists in the list of users (experts, customer). 
        /// The data comes from mobile clients.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Task<OperationResult<ValidatedUser>> AuthorizeUserAsync(LoginViewModel data) {
            var result = OperationResult.CreateResult<ValidatedUser>();
            if (data.Password == "p@ssword" && data.UserName.ToLowerInvariant() == "administrator") {
                result.Result = new ValidatedUser { UserName = data.UserName, Claims = CreateClaimsList(data) };
                return Task.FromResult(result);
            }
            result.AddError($"Извините, {data.UserName} не существует в системе.");
            return Task.FromResult(result);
        }

        private IEnumerable<Claim> CreateClaimsList<T>(T entity, IEnumerable<Claim> additionalClaims = null) {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            var result = new List<Claim>();
            if (additionalClaims != null) result.AddRange(additionalClaims);
            var properties = typeof(T).GetProperties().Where(t => t.PropertyType.IsPrimitive 
                    || t.PropertyType.IsValueType || (t.PropertyType == typeof(string)));
            var items = from property in properties
                        let value = property.GetValue(entity)
                        where value != null
                        select new Claim(property.Name, value?.ToString());
            result.AddRange(items);
            return result;
        }
    }
}