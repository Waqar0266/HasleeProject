namespace Hasslefree.Core.Data
{
	public interface IConnectionStringResolver
	{
		string ConnectionString { get; }
		string DatabaseName { get; }
		string ServerName { get; }
		IConnectionStringResolver WithName(string contextName);
	}
}
