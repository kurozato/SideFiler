using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackSugar.SimpleMvp
{
    public abstract class Validator<TViewModel> : IValidator<TViewModel>
    {
        List<(Func<TViewModel, bool> expression, Action<TViewModel> errAction, string[] tags)> Rules;

        public Validator()
        {
            Rules = new List<(Func<TViewModel, bool> expression, Action<TViewModel> errAction, string[] tags)>();
        }

        /// <summary>
        /// validate all rules
        /// </summary>
        /// <param name="view"></param>
        /// <returns>
        /// ok -&gt; true, fail -&gt; false
        /// </returns>
        public bool ValidateAll(TViewModel view)
        {
            var isValid = true;
            foreach (var rule in Rules)
            {
                if (!rule.expression(view))
                {
                    rule.errAction(view);
                    isValid = false;
                }        
            }
            return isValid;
        }

        /// <summary>
        /// validate rules for tag
        /// </summary>
        /// <param name="view"></param>
        /// <param name="tag"></param>
        /// <returns>
        /// ok -&gt; true, fail -&gt; false
        /// </returns>
        public bool Validate(TViewModel view, string tag)
        {
            var isValid = true;
            foreach (var rule in Rules.Where(r => r.tags.Any(t => t.Equals(tag))))
            {
                if (!rule.expression(view))
                {
                    rule.errAction(view);
                    isValid = false;
                }
            }
            return isValid;

        }

        /// <summary>
        /// when expression is false, execute errAction
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="errAction"></param>
        /// <param name="tags"></param>
        protected void Rule(Func<TViewModel, bool> expression, Action<TViewModel> errAction, params string[] tags)
        {
            Rules.Add((expression, errAction, tags));
        }
    }
}
