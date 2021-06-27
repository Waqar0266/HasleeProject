namespace Hasslefree.Services.Forms
{
	public interface IFillFormService
	{
		IFillFormService Prepare(string formName);
		IFillFormService WithField(string fieldName, string fieldValue);
		IFillFormService WithCheckbox(string checkboxName, bool check);
		IFillFormService WithImage(byte[] image, int pageNumber, int x, int y, int height, int width);
		byte[] Process();
	}
}
