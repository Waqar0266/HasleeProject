using Hasslefree.Core;
using Hasslefree.Core.Domain.Rentals;
using Hasslefree.Core.Helpers.Extensions;
using Hasslefree.Core.Logging;
using Hasslefree.Core.Sessions;
using Hasslefree.Services.Accounts.Actions;
using Hasslefree.Services.Common;
using Hasslefree.Services.Emails;
using Hasslefree.Services.Forms;
using Hasslefree.Services.Media.Downloads;
using Hasslefree.Services.Media.Pictures;
using Hasslefree.Services.RentalForms;
using Hasslefree.Services.Rentals.Crud;
using Hasslefree.Web.Framework;
using Hasslefree.Web.Framework.Filters;
using Hasslefree.Web.Models.Rentals;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Hasslefree.Business.Controllers.Rentals
{
    public class CompleteRentalLandlordSignatureController : BaseController
    {
        #region Private Properties 

        // Services
        private IUpdateRentalService UpdateRentalService { get; }
        private IUpdateRentalLandlordService UpdateRentalLandlordService { get; }
        private IUploadPictureService UploadPicture { get; }
        private IUploadDownloadService UploadDownload { get; }
        private IFillFormService FillForm { get; }
        private IGetFirmService GetFirmService { get; }
        private ICreateRentalFormService CreateRentalForm { get; }
        private ILogoutService LogoutService { get; }
        private ISendMail SendMail { get; }
        private IUpdateRentalWitnessService UpdateRentalWitnessService { get; }
        private IGetRentalService GetRental { get; }
        private IUpdateRentalFicaService UpdateRentalFicaService { get; }
        private IUpdateRentalResolutionMemberService UpdateRentalResolutionMemberService { get; }

        // Other
        private IWebHelper WebHelper { get; }
        private ISessionManager SessionManager { get; }

        #endregion

        #region Constructor

        public CompleteRentalLandlordSignatureController
        (
            //Services
            IUpdateRentalService updateRentalService,
            IUploadPictureService uploadPicture,
            IUploadDownloadService uploadDownload,
            IFillFormService fillForm,
            IGetFirmService getFirmService,
            ICreateRentalFormService createRentalForm,
            ILogoutService logoutService,
            ISendMail sendMail,
            IUpdateRentalLandlordService updateRentalLandlordService,
            IUpdateRentalWitnessService updateRentalWitnessService,
            IGetRentalService getRental,
            IUpdateRentalFicaService updateRentalFicaService,
            IUpdateRentalResolutionMemberService updateRentalResolutionMemberService,

            //Other
            IWebHelper webHelper,
            ISessionManager sessionManager
        )
        {
            // Services
            UpdateRentalService = updateRentalService;
            UploadPicture = uploadPicture;
            UploadDownload = uploadDownload;
            FillForm = fillForm;
            GetFirmService = getFirmService;
            CreateRentalForm = createRentalForm;
            LogoutService = logoutService;
            SendMail = sendMail;
            UpdateRentalLandlordService = updateRentalLandlordService;
            UpdateRentalWitnessService = updateRentalWitnessService;
            GetRental = getRental;
            UpdateRentalFicaService = updateRentalFicaService;
            UpdateRentalResolutionMemberService = updateRentalResolutionMemberService;

            // Other
            WebHelper = webHelper;
            SessionManager = sessionManager;
        }

        #endregion

        #region Actions

        [HttpGet, Route("account/rental/l/complete-signature")]
        [AccessControlFilter]
        public ActionResult CompleteLandlordSignature(string hash)
        {
            string decodedHash = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(hash));

            int rentalId = Int32.Parse(decodedHash.Split(';')[0]);
            string landlordUniqueId = decodedHash.Split(';')[1];

            var rental = GetRental[rentalId].Get();

            if (rental.RentalStatus != RentalStatus.PendingLandlordSignature) return Redirect($"/account/rentals");

            var model = new CompleteRentalLandlordSignature
            {
                RentalId = rentalId,
                LandlordGuid = decodedHash.Split(';')[1]
            };

            ViewBag.Title = "Complete Landlord Signature";

            // Ajax
            if (WebHelper.IsAjaxRequest()) return PartialView("../Rentals/CompleteSignature", model);

            // Default
            return View("../Rentals/CompleteSignature", model);
        }

        [HttpGet, Route("account/rental/l/complete-witness-signature")]
        public ActionResult CompleteLandlordWitnessSignature(string hash)
        {
            if (SessionManager.IsLoggedIn())
            {
                LogoutService.Logout();
                return Redirect($"/account/rental/l/complete-witness-signature?hash={hash}");
            }

            var decodedHash = Encoding.UTF8.GetString(Convert.FromBase64String(hash));

            var rentalUniqueId = decodedHash.Split(';')[0];
            var uniqueId = decodedHash.Split(';')[1];
            var witnessNumber = Int32.Parse(decodedHash.Split(';')[2]);

            var rental = GetRental[rentalUniqueId].Get();

            if (witnessNumber == 1 && rental.RentalWitness.LandlordWitness1SignatureId.HasValue && rental.RentalWitness.LandlordWitness1InitialsId.HasValue) return Redirect("/account/rental/l/complete-witness-signature/success");
            if (witnessNumber == 2 && rental.RentalWitness.LandlordWitness2SignatureId.HasValue && rental.RentalWitness.LandlordWitness2InitialsId.HasValue) return Redirect("/account/rental/l/complete-witness-signature/success");

            var model = new CompleteRentalWitnessLandlordSignature
            {
                UniqueId = uniqueId,
                WitnessNumber = witnessNumber,
                RentalId = rental.RentalId
            };

            ViewBag.Title = "Complete Witness Signature";

            // Ajax
            if (WebHelper.IsAjaxRequest()) return PartialView("../Rentals/CompleteWitnessSignature", model);

            // Default
            return View("../Rentals/CompleteWitnessSignature", model);
        }

        [HttpGet, Route("account/rental/l/complete-member-signature")]
        public ActionResult CompleteMemberSignature(string hash)
        {
            if (SessionManager.IsLoggedIn())
            {
                LogoutService.Logout();
                return Redirect($"/account/rental/l/complete-member-signature?hash={hash}");
            }

            var decodedHash = Encoding.UTF8.GetString(Convert.FromBase64String(hash));

            var rentalId = Int32.Parse(decodedHash.Split(';')[0]);
            var rentalResolutionMemberId = Int32.Parse(decodedHash.Split(';')[1]);

            var rental = GetRental[rentalId].Get();

            var rentalResolutionMember = rental.RentalResolution.Members.FirstOrDefault(a => a.RentalResolutionMemberId == rentalResolutionMemberId);

            if (rentalResolutionMember.SignatureId.HasValue) return Redirect("/account/rental/l/complete-member-signature/success");

            var model = new CompleteRentalResolutionMemberSignature
            {
                RentalResolutionMemberId = rentalResolutionMemberId,
                RentalId = rental.RentalId
            };

            ViewBag.Title = "Complete Member Signature";

            // Ajax
            if (WebHelper.IsAjaxRequest()) return PartialView("../Rentals/CompleteMemberSignature", model);

            // Default
            return View("../Rentals/CompleteMemberSignature", model);
        }

        [HttpPost, Route("account/rental/l/complete-member-signature")]
        public ActionResult CompleteMemberSignature(CompleteRentalResolutionMemberSignature model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var rental = GetRental[model.RentalId].Get();
                    var rentalResolutionMember = rental.RentalResolution.Members.FirstOrDefault(a => a.RentalResolutionMemberId == model.RentalResolutionMemberId);

                    //add the signatures
                    UploadPicture.WithPath($"signatures/rental/{rental.RentalId}/partners");

                    var signatureData = RemoveWhitespace(model.Signature);

                    string name = rentalResolutionMember.Name;
                    string surname = rentalResolutionMember.Surname;

                    UploadPicture.Add(new Web.Models.Media.Pictures.Crud.PictureModel()
                    {
                        Action = Web.Models.Common.CrudAction.Create,
                        File = signatureData,
                        Format = Core.Domain.Media.PictureFormat.Png,
                        Key = $"{name.ToLower().Replace(" ", "-")}_{surname.ToLower().Replace(" ", "-")}_signature.png",
                        Name = $"{name.ToLower().Replace(" ", "-")}_{surname.ToLower().Replace(" ", "-")}_signature.png",
                        MimeType = "image/png",
                        AlternateText = $"{name.ToLower().Replace(" ", "-")}_{surname.ToLower().Replace(" ", "-")}_signature.jpg"
                    });

                    var pictures = UploadPicture.Save();

                    bool success = UpdateRentalResolutionMemberService.WithRentalId(rental.RentalId)[model.RentalResolutionMemberId]
                    .Set(a => a.SignatureId, pictures.FirstOrDefault().PictureId)
                    .Set(a => a.SignedAt, model.SignedAt)
                    .Set(a => a.SignedOn, DateTime.Now)
                    .Update();

                    rental = GetRental[model.RentalId].Get();

                    // Success
                    if (success)
                    {
                        //verify if all the partners signed
                        bool allSigned = rental.RentalResolution.Members.All(a => a.SignatureId.HasValue);

                        if (allSigned)
                        {
                            success = UpdateRentalService[rental.RentalId]
                            .Set(a => a.ModifiedOn, DateTime.Now)
                            .Set(a => a.RentalStatus, RentalStatus.PendingLandlordWitnessSignature)
                            .Update();

                            SendLandlordWitnessEmail(rental.RentalWitness.LandlordWitness1Email, rental.RentalWitness.RentalWitnessId, rental.RentalId, 1);
                            SendLandlordWitnessEmail(rental.RentalWitness.LandlordWitness1Email, rental.RentalWitness.RentalWitnessId, rental.RentalId, 2);
                        }

                        // Ajax (+ Json)
                        if (WebHelper.IsAjaxRequest() || WebHelper.IsJsonRequest()) return Json(new
                        {
                            Success = true,
                            AgentId = 1,
                        }, JsonRequestBehavior.AllowGet);

                        // Default
                        return Redirect($"/account/rental/l/complete-member-signature/success");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                while (ex.InnerException != null) ex = ex.InnerException;
                ModelState.AddModelError("", ex.Message);
            }

            var errors = "";

            //if (UpdateAgentService.HasWarnings) UpdateAgentService.Warnings.ForEach(w => errors += w.Message + "\n");
            //if (CreateAgentForm.HasWarnings) CreateAgentForm.Warnings.ForEach(w => errors += w.Message + "\n");

            ModelState.AddModelError("", errors);

            ViewBag.Title = "Complete Agent Signature";

            // Ajax (Json)
            if (WebHelper.IsJsonRequest()) return Json(new
            {
                Success = false,
                Message = errors ?? "Unexpected error has occurred."
            }, JsonRequestBehavior.AllowGet);

            // Ajax
            if (WebHelper.IsAjaxRequest()) return PartialView("../Agents/CompleteSignature", model);

            // Default
            return View("../Agents/CompleteSignature", model);
        }

        [HttpGet, Route("account/rental/l/complete-member-signature/success")]
        public ActionResult CompleteMemberSignatureSuccess()
        {
            if (SessionManager.IsLoggedIn())
            {
                LogoutService.Logout();
                return Redirect($"/account/rental/l/complete-member-signature/success");
            }

            ViewBag.Title = "Completed Member Signature";

            // Ajax
            if (WebHelper.IsAjaxRequest()) return PartialView("../Rentals/CompleteMemberSignatureSuccess");

            // Default
            return View("../Rentals/CompleteMemberSignatureSuccess");
        }

        [HttpGet, Route("account/rental/l/complete-witness-signature/success")]
        public ActionResult CompleteLandlordWitnessSignatureSuccess()
        {
            if (SessionManager.IsLoggedIn())
            {
                LogoutService.Logout();
                return Redirect($"/account/rental/complete-witness-signature/success");
            }

            ViewBag.Title = "Completed Witness Signature";

            // Ajax
            if (WebHelper.IsAjaxRequest()) return PartialView("../Rentals/CompleteWitnessSignatureSuccess");

            // Default
            return View("../Rentals/CompleteWitnessSignatureSuccess");
        }

        [HttpPost, Route("account/rental/l/complete-signature")]
        public ActionResult CompleteLandlordSignature(CompleteRentalLandlordSignature model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var rental = GetRental[model.RentalId].Get();

                    var landlord = rental.RentalLandlords.FirstOrDefault(a => a.UniqueId.ToString().ToLower() == model.LandlordGuid.ToLower());

                    var person = landlord.Person;

                    //add the signatures
                    UploadPicture.WithPath("signatures");

                    var signatureData = RemoveWhitespace(model.Signature);

                    var key = $"{person.FirstName.ToLower().Replace(" ", "-")}_{person.Surname.ToLower().Replace(" ", "-")}_{model.LandlordGuid}";

                    UploadPicture.Add(new Web.Models.Media.Pictures.Crud.PictureModel()
                    {
                        Action = Web.Models.Common.CrudAction.Create,
                        File = signatureData,
                        Format = Core.Domain.Media.PictureFormat.Png,
                        Key = $"{key}_signature.png",
                        Name = $"{key}_signature.png",
                        MimeType = "image/png",
                        AlternateText = $"{key}_signature.png"
                    });

                    UploadPicture.Add(new Web.Models.Media.Pictures.Crud.PictureModel()
                    {
                        Action = Web.Models.Common.CrudAction.Create,
                        File = RemoveWhitespace(model.Initials),
                        Format = Core.Domain.Media.PictureFormat.Png,
                        Key = $"{key}_initial.png",
                        Name = $"{key}_initial.png",
                        MimeType = "image/png",
                        AlternateText = $"{key}_initial.png"
                    });

                    var pictures = UploadPicture.Save();

                    var success = UpdateRentalLandlordService[landlord.RentalLandlordId]
                    .Set(a => a.SignedAt, model.SignedAtSignature)
                    .Set(a => a.SignedOn, DateTime.Now)
                    .Set(a => a.SignatureId, pictures.FirstOrDefault(p => p.Name == $"{key}_signature.png").PictureId)
                    .Set(a => a.InitialsId, pictures.FirstOrDefault(p => p.Name == $"{key}_initial.png").PictureId)
                    .Update();

                    //add the witnesses to the database
                    int rentalWitnessId = 0;
                    if (rental.RentalWitness != null) rentalWitnessId = rental.RentalWitness.RentalWitnessId;
                    success = UpdateRentalWitnessService.WithRentalId(rental.RentalId)[rentalWitnessId]
                    .Set(r => r.LandlordWitness1Email, model.Witness1Email)
                    .Set(r => r.LandlordWitness1Name, model.Witness1Name)
                    .Set(r => r.LandlordWitness1Surname, model.Witness1Surname)
                    .Set(r => r.LandlordWitness1Mobile, model.Witness1Mobile)
                    .Set(r => r.LandlordWitness2Email, model.Witness2Email)
                    .Set(r => r.LandlordWitness2Name, model.Witness2Name)
                    .Set(r => r.LandlordWitness2Surname, model.Witness2Surname)
                    .Set(r => r.LandlordWitness2Mobile, model.Witness2Mobile)
                    .Update();

                    rentalWitnessId = UpdateRentalWitnessService.RentalWitnessId;

                    if (rental.LeaseType == LeaseType.Natural && rental.RentalFica == null && String.IsNullOrEmpty(rental.RentalFica?.Partner1Name) && String.IsNullOrEmpty(rental.RentalFica?.Partner2Name) && String.IsNullOrEmpty(rental.RentalFica?.Partner3Name))
                    {
                        success = UpdateRentalService[rental.RentalId]
                        .Set(a => a.RentalStatus, RentalStatus.PendingLandlordWitnessSignature)
                        .Update();

                        SendLandlordWitnessEmail(model.Witness1Email, rentalWitnessId, rental.RentalId, 1);
                        SendLandlordWitnessEmail(model.Witness2Email, rentalWitnessId, rental.RentalId, 2);
                    }
                    else
                    {
                        foreach (var member in rental.RentalResolution.Members)
                            SendMemberSignatureEmail(member.Email, rental.RentalId, member.RentalResolutionMemberId);

                        success = UpdateRentalService[rental.RentalId]
                        .Set(a => a.RentalStatus, RentalStatus.PendingMemberSignatures)
                        .Update();
                    }

                    // Success
                    if (success)
                    {
                        // Ajax (+ Json)
                        if (WebHelper.IsAjaxRequest() || WebHelper.IsJsonRequest()) return Json(new
                        {
                            Success = true,
                            AgentId = 1,
                        }, JsonRequestBehavior.AllowGet);

                        // Default
                        return Redirect($"/account/rentals");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                while (ex.InnerException != null) ex = ex.InnerException;
                ModelState.AddModelError("", ex.Message);
            }

            var errors = "";

            //if (UpdateAgentService.HasWarnings) UpdateAgentService.Warnings.ForEach(w => errors += w.Message + "\n");
            //if (CreateAgentForm.HasWarnings) CreateAgentForm.Warnings.ForEach(w => errors += w.Message + "\n");

            ModelState.AddModelError("", errors);

            ViewBag.Title = "Complete Agent Signature";

            // Ajax (Json)
            if (WebHelper.IsJsonRequest()) return Json(new
            {
                Success = false,
                Message = errors ?? "Unexpected error has occurred."
            }, JsonRequestBehavior.AllowGet);

            // Ajax
            if (WebHelper.IsAjaxRequest()) return PartialView("../Agents/CompleteSignature", model);

            // Default
            return View("../Agents/CompleteSignature", model);
        }

        [HttpPost, Route("account/rental/l/complete-witness-signature")]
        public ActionResult CompleteLandlordWitnessSignature(CompleteRentalWitnessLandlordSignature model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var rental = GetRental[model.RentalId].Get();

                    //add the signatures
                    UploadPicture.WithPath($"signatures/rental/{rental.RentalId}");

                    var signatureData = RemoveWhitespace(model.Signature);

                    string name = model.WitnessNumber == 1 ? rental.RentalWitness.LandlordWitness1Name : rental.RentalWitness.LandlordWitness2Name;
                    string surname = model.WitnessNumber == 2 ? rental.RentalWitness.LandlordWitness1Surname : rental.RentalWitness.LandlordWitness2Surname;

                    UploadPicture.Add(new Web.Models.Media.Pictures.Crud.PictureModel()
                    {
                        Action = Web.Models.Common.CrudAction.Create,
                        File = signatureData,
                        Format = Core.Domain.Media.PictureFormat.Png,
                        Key = $"{name.ToLower().Replace(" ", "-")}_{surname.ToLower().Replace(" ", "-")}_signature.png",
                        Name = $"{name.ToLower().Replace(" ", "-")}_{surname.ToLower().Replace(" ", "-")}_signature.png",
                        MimeType = "image/png",
                        AlternateText = $"{name.ToLower().Replace(" ", "-")}_{surname.ToLower().Replace(" ", "-")}_signature.jpg"
                    });

                    UploadPicture.Add(new Web.Models.Media.Pictures.Crud.PictureModel()
                    {
                        Action = Web.Models.Common.CrudAction.Create,
                        File = RemoveWhitespace(model.Initials),
                        Format = Core.Domain.Media.PictureFormat.Png,
                        Key = $"{name.ToLower().Replace(" ", "-")}_{surname.ToLower().Replace(" ", "-")}_initial.png",
                        Name = $"{name.ToLower().Replace(" ", "-")}_{surname.ToLower().Replace(" ", "-")}_initial.png",
                        MimeType = "image/png",
                        AlternateText = $"{name.ToLower().Replace(" ", "-")}_{surname.ToLower().Replace(" ", "-")}_initial"
                    });

                    var pictures = UploadPicture.Save();
                    bool success = false;

                    if (model.WitnessNumber == 1)
                    {
                        success = UpdateRentalWitnessService.WithRentalId(rental.RentalWitness.RentalId)[rental.RentalWitness.RentalWitnessId]
                        .Set(a => a.LandlordWitness1SignedAt, model.SignedAtSignature)
                        .Set(a => a.LandlordWitness1SignedOn, DateTime.Now)
                        .Set(a => a.LandlordWitness1SignatureId, pictures.FirstOrDefault(p => p.Name == $"{name.ToLower().Replace(" ", "-")}_{surname.ToLower().Replace(" ", "-")}_signature.png").PictureId)
                        .Set(a => a.LandlordWitness1InitialsId, pictures.FirstOrDefault(p => p.Name == $"{name.ToLower().Replace(" ", "-")}_{surname.ToLower().Replace(" ", "-")}_initial.png").PictureId)
                        .Update();
                    }

                    if (model.WitnessNumber == 2)
                    {
                        success = UpdateRentalWitnessService.WithRentalId(rental.RentalWitness.RentalId)[rental.RentalWitness.RentalWitnessId]
                        .Set(a => a.LandlordWitness2SignedAt, model.SignedAtSignature)
                        .Set(a => a.LandlordWitness2SignedOn, DateTime.Now)
                        .Set(a => a.LandlordWitness2SignatureId, pictures.FirstOrDefault(p => p.Name == $"{name.ToLower().Replace(" ", "-")}_{surname.ToLower().Replace(" ", "-")}_signature.png").PictureId)
                        .Set(a => a.LandlordWitness2InitialsId, pictures.FirstOrDefault(p => p.Name == $"{name.ToLower().Replace(" ", "-")}_{surname.ToLower().Replace(" ", "-")}_initial.png").PictureId)
                        .Update();
                    }

                    int rentalWitnessId = UpdateRentalWitnessService.RentalWitnessId;

                    rental = GetRental[model.RentalId].Get();

                    // Success
                    if (success)
                    {
                        //verify if landlord witnesses signed
                        if (rental.RentalWitness.LandlordWitness1SignatureId.HasValue && rental.RentalWitness.LandlordWitness1InitialsId.HasValue && rental.RentalWitness.LandlordWitness2SignatureId.HasValue && rental.RentalWitness.LandlordWitness2InitialsId.HasValue)
                        {
                            SendAgentSignatureEmail(rental.AgentPerson.Email, rental.RentalId);
                            UpdateRentalService[rental.RentalId]
                            .Set(a => a.ModifiedOn, DateTime.Now)
                            .Set(a => a.RentalStatus, RentalStatus.PendingAgentSignature)
                            .Update();
                        }

                        // Ajax (+ Json)
                        if (WebHelper.IsAjaxRequest() || WebHelper.IsJsonRequest()) return Json(new
                        {
                            Success = true,
                            AgentId = 1,
                        }, JsonRequestBehavior.AllowGet);

                        // Default
                        return Redirect($"/account/rental/l/complete-witness-signature/success");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                while (ex.InnerException != null) ex = ex.InnerException;
                ModelState.AddModelError("", ex.Message);
            }

            var errors = "";

            //if (UpdateAgentService.HasWarnings) UpdateAgentService.Warnings.ForEach(w => errors += w.Message + "\n");
            //if (CreateAgentForm.HasWarnings) CreateAgentForm.Warnings.ForEach(w => errors += w.Message + "\n");

            ModelState.AddModelError("", errors);

            ViewBag.Title = "Complete Agent Signature";

            // Ajax (Json)
            if (WebHelper.IsJsonRequest()) return Json(new
            {
                Success = false,
                Message = errors ?? "Unexpected error has occurred."
            }, JsonRequestBehavior.AllowGet);

            // Ajax
            if (WebHelper.IsAjaxRequest()) return PartialView("../Agents/CompleteSignature", model);

            // Default
            return View("../Agents/CompleteSignature", model);
        }

        #endregion

        #region Private Methods

        private static byte[] RemoveWhitespace(string base64)
        {
            Bitmap bmp;
            byte[] imageAsBytes = Convert.FromBase64String(base64.Replace("data:image/png;base64,", ""));
            using (var ms = new MemoryStream(imageAsBytes))
            {
                var image = Bitmap.FromStream(ms);
                bmp = new Bitmap(image);
            }

            int w = bmp.Width;
            int h = bmp.Height;

            Func<int, bool> allWhiteRow = row =>
            {
                for (int i = 0; i < w; ++i)
                {
                    if (bmp.GetPixel(i, row).R != 255)
                        return false;
                }
                return true;
            };

            Func<int, bool> allWhiteColumn = col =>
            {
                for (int i = 0; i < h; ++i)
                {
                    if (bmp.GetPixel(col, i).R != 255)
                        return false;
                }
                return true;
            };

            int topmost = 0;
            for (int row = 0; row < h; ++row)
            {
                if (allWhiteRow(row))
                    topmost = row;
                else break;
            }

            int bottommost = 0;
            for (int row = h - 1; row >= 0; --row)
            {
                if (allWhiteRow(row))
                    bottommost = row;
                else break;
            }

            int leftmost = 0, rightmost = 0;
            for (int col = 0; col < w; ++col)
            {
                if (allWhiteColumn(col))
                    leftmost = col;
                else
                    break;
            }

            for (int col = w - 1; col >= 0; --col)
            {
                if (allWhiteColumn(col))
                    rightmost = col;
                else
                    break;
            }

            if (rightmost == 0) rightmost = w; // As reached left
            if (bottommost == 0) bottommost = h; // As reached top.

            int croppedWidth = rightmost - leftmost;
            int croppedHeight = bottommost - topmost;

            if (croppedWidth == 0) // No border on left or right
            {
                leftmost = 0;
                croppedWidth = w;
            }

            if (croppedHeight == 0) // No border on top or bottom
            {
                topmost = 0;
                croppedHeight = h;
            }

            try
            {
                var target = new Bitmap(croppedWidth, croppedHeight);
                using (Graphics g = Graphics.FromImage(target))
                {
                    g.DrawImage(bmp,
                      new RectangleF(0, 0, croppedWidth, croppedHeight),
                      new RectangleF(leftmost, topmost, croppedWidth, croppedHeight),
                      GraphicsUnit.Pixel);
                }

                target.MakeTransparent();

                return target.ToByteArray(System.Drawing.Imaging.ImageFormat.Png);
            }
            catch (Exception ex)
            {
                throw new Exception(
                  string.Format("Values are topmost={0} btm={1} left={2} right={3} croppedWidth={4} croppedHeight={5}", topmost, bottommost, leftmost, rightmost, croppedWidth, croppedHeight),
                  ex);
            }
        }

        private bool SendLandlordWitnessEmail(string email, int rentalWitnessId, int rentalId, int witnessNumber)
        {
            var url = $"account/rental/emails/landlord-witness-email?witnessNumber={witnessNumber}&rentalId={rentalId}&witnessId={rentalWitnessId}";

            SendMail.WithUrlBody(url).WithRecipient(email);

            return SendMail.Send("New Listing - Landlord Witness Signature");
        }

        private bool SendAgentSignatureEmail(string email, int rentalId)
        {
            var url = $"account/rental/emails/agent-signature-email?rentalId={rentalId}";

            SendMail.WithUrlBody(url).WithRecipient(email);

            return SendMail.Send("New Listing - Agent Signature");
        }

        private bool SendMemberSignatureEmail(string email, int rentalId, int rentalResolutionMemberId)
        {
            var url = $"account/rental/emails/member-signature-email?rentalId={rentalId}&rentalResolutionMemberId={rentalResolutionMemberId}";

            SendMail.WithUrlBody(url).WithRecipient(email);

            return SendMail.Send("New Listing - Member Signature");
        }

        #endregion
    }
}