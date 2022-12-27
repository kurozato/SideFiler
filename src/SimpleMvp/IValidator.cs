namespace BlackSugar.SimpleMvp
{
    public interface IValidator<TViewModel>
    {
        /// <summary>
        /// validate all rules
        /// </summary>
        /// <param name="view"></param>
        /// <returns>
        /// ok -&gt; true, fail -&gt; false
        /// </returns>
        bool ValidateAll(TViewModel view);

        /// <summary>
        /// validate rules for tag
        /// </summary>
        /// <param name="view"></param>
        /// <param name="tag"></param>
        /// <returns>
        /// ok -&gt; true, fail -&gt; false
        /// </returns>
        bool Validate(TViewModel view, string tag);
    }
}
