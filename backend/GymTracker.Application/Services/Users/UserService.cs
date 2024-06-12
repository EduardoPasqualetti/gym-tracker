using GymTracker.Application.Services.Contracts.Requests;
using GymTracker.Application.Services.Contracts.Responses;
using GymTracker.Domain.Entities;
using GymTracker.Domain.Repositories;
using GymTracker.Infra.CloudStorage;
using GymTracker.Infra.Data;
using GymTracker.Infra.Data.DAOs.ProfileHistory;
using GymTracker.Infra.Data.DAOs.User;
using GymTracker.Infra.Data.DAOs.UserLike;
using GymTracker.Infra.Data.UnityOfWork;
using GymTracker.Utils.Cryptography;
using GymTracker.Utils.DTOs;
using GymTracker.Utils.Token;
using Microsoft.EntityFrameworkCore;

namespace GymTracker.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUnityOfWork _unityOfWork;

    private readonly IProfileHistoryDAO _profileHistoryDAO;
    private readonly IUserDAO _userDAO;
    private readonly IUserLikeDAO _userLikeDAO;

    private readonly ICryptographyStrategy _cryptographyStrategy;
    private readonly ITokenStrategy _tokenStrategy;

    private readonly ICloudStorage _cloudStorage;

    private readonly DataContext _context;

    public UserService(IUserRepository userRepository, IUnityOfWork unityOfWork, ICryptographyStrategy cryptographyStrategy, IProfileHistoryDAO profileHistoryDAO, IUserDAO userDAO, IUserLikeDAO userLikeDAO, ITokenStrategy tokenStrategy, ICloudStorage cloudStorage, DataContext context)
    {
        _userRepository = userRepository;
        _unityOfWork = unityOfWork;

        _profileHistoryDAO = profileHistoryDAO;
        _userDAO = userDAO;
        _userLikeDAO = userLikeDAO;

        _cryptographyStrategy = cryptographyStrategy;
        _tokenStrategy = tokenStrategy;

        _cloudStorage = cloudStorage;

        _context = context;
    }

    public async Task<RegisterUserResponse> RegisterUser(RegisterUserRequest request)
    {
        User user = new User(
            email: request.email,
            plainPassword: request.password,
            name: request.name,
            birthYear: request.birthYear,
            gender: request.gender,
            cryptographyStrategy: _cryptographyStrategy!
        );

        await _userRepository!.CreateUser(user);
        await _unityOfWork!.Commit();

        var response = new RegisterUserResponse(
            name: user.Name!,
            email: user.Email!
        );

        return response;
    }

    public async Task<LoginResponse> Login(LoginRequest request)
    {
        try
        {
            UserLoginDTO findedUser = await _userDAO.GetUserByEmail(request.userEmail);

            if (findedUser == null)
                throw new Exception("Dados inválidos.");

            var hashMatching = _cryptographyStrategy.VerifyHashedPassword(request.userPassword, findedUser.userHash, findedUser.userSalt);

            if (!hashMatching)
                throw new Exception("Dados inválidos.");

            var loginToken = _tokenStrategy.GenerateToken(findedUser);

            return new LoginResponse(loginToken);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<RegisterProfileHistoryResponse> RegisterProfileHistory(RegisterProfileHistoryRequest request)
    {
        try
        {
            var user = await _userRepository.GetUserById(request.userId);

            user.MarkProfileAsUpdated();

            string? newEvolutionPhotoUri = null;

            if (request.evolutionPhoto != null)
            {
                newEvolutionPhotoUri = await _cloudStorage.UploadData(request.evolutionPhoto);
            }

            var lastProfileHistory = await _profileHistoryDAO.GetLastProfileHistoryByUserId(request.userId);

            ProfileHistory profileHistory;

            if (lastProfileHistory != null)
            {
                profileHistory = new ProfileHistory(
                    lastHistoryDate: lastProfileHistory.profileDate,
                    userId: request.userId,
                    weight: request.weight,
                    height: request.height,
                    abdominalGirth: request.abdominalGirth,
                    scapularGirth: request.scapularGirth,
                    hipGirth: request.hipGirth,
                    armGirth: request.armGirth,
                    legGirth: request.armGirth,
                    bodyFat: request.bodyFat,
                    evolutionPhoto: newEvolutionPhotoUri
                );
            }
            else
            {
                profileHistory = new ProfileHistory(
                    userId: request.userId,
                    weight: request.weight,
                    height: request.height,
                    abdominalGirth: request.abdominalGirth,
                    scapularGirth: request.scapularGirth,
                    hipGirth: request.hipGirth,
                    armGirth: request.armGirth,
                    legGirth: request.armGirth,
                    bodyFat: request.bodyFat,
                    evolutionPhoto: newEvolutionPhotoUri
                );
            }


            await _userRepository.CreateUserProfileHistory(profileHistory);
            await _unityOfWork.Commit();

            var response = new RegisterProfileHistoryResponse(
                profileHistoryId: profileHistory.ProfileHistoryId,
                userId: profileHistory.UserId,
                weight: profileHistory.Weight,
                height: profileHistory.Height,
                abdominalGirth: profileHistory.AbdominalGirth,
                scapularGirth: profileHistory.ScapularGirth,
                hipGirth: profileHistory.HipGirth,
                armGirth: profileHistory.ArmGirth,
                legGirth: profileHistory.LegGirth,
                bodyFat: profileHistory.BodyFat,
                evolutionPhoto: profileHistory.EvolutionPhoto
            );

            return response;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<IEnumerable<ProfileHistoryDTO>> ListProfileHistoryByUserId(Guid userId)
    {
        try
        {
            var profileHistoryList = await _profileHistoryDAO.ListProfileHistoriesByUserId(userId);
            return profileHistoryList;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<ChangeUserProfileImageResponse> ChangeUserProfileImage(ChangeUserProfileImageRequest request)
    {
        try
        {
            if (request.profileImageFile == null)
                throw new ArgumentException("Profile image file should not be empty.");

            var user = await _userRepository.GetUserById(request.userId);
            string newProfilePhotoUri = await _cloudStorage.UploadData(request.profileImageFile);

            user.SetProfilePhoto(newProfilePhotoUri);

            await _unityOfWork.Commit();

            var response = new ChangeUserProfileImageResponse(
                profileImageUri: user.ProfilePhoto!
            );

            return response;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public async Task<RegisterUserLikeResponse> RegisterUserLike(RegisterUserLikeRequest request)
    {
        try
        {
            var userLike = new UserLike(
                senderUserId: request.senderUserId,
                receiverUserId: request.receiverUserId
            );

            await _userRepository.CreateUserLike(userLike);
            await _unityOfWork.Commit();

            var response = new RegisterUserLikeResponse(
                userLikeId: userLike.UserLikeId,
                senderUserId: userLike.SenderUserId,
                receiverUserId: userLike.ReceiverUserId
            );

            return response;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task RemoveUserLike(Guid userLikeId)
    {
        try
        {
            var userLike = await _userRepository.GetUserLikeById(userLikeId);

            if (userLike == null)
                throw new Exception("User like not found");

            _userRepository.RemoveUserLike(userLike);
            await _unityOfWork.Commit();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public Task<int> GetLikesByUserID(Guid userId)
    {
        try
        {
            var likesAmount = _userLikeDAO.LikesByUserId(userId);

            return likesAmount;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<CanAddProfileHistoryResponse> CanAddProfileHistory(Guid userId)
    {
        try
        {
            bool havePermition = true;

            var lastProfileHistoryDTO = await _profileHistoryDAO.GetLastProfileHistoryByUserId(userId);

            if (lastProfileHistoryDTO != null)
            {
                var lastProfileHistory = await _userRepository.GetUserProfileHistoryById(lastProfileHistoryDTO.profileHistoryId);
                System.Console.WriteLine(lastProfileHistory.ProfileDate);

                havePermition = lastProfileHistory.IsValidSpaceBetweenDates(lastProfileHistory.ProfileDate);
            }

            var response = new CanAddProfileHistoryResponse(havePermition);

            return response;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<IEnumerable<RankUserDTO>> ListRankedUsersByLikesAmount()
    {
        try
        {
            return await _userDAO.RankUsersByLikes();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<IEnumerable<RankUserDTO>> ListRankedUsersByLastProfileUpdate()
    {
        try
        {
            return await _userDAO.RankUsersByLastProfileUpdate();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<ChangePasswordResponse> ChangePassword(ChangePasswordRequest request)
    {
        var user = await _userRepository.GetUserByEmail(request.userEmail);

        if (user == null)
        {
            return new ChangePasswordResponse(false, "User not found.");
        }

        var isPasswordChanged = user.ChangePassword(request.newPassword, request.passwordRecoverCode, _cryptographyStrategy);

        if (isPasswordChanged)
        {
            // Atualize o usuário diretamente usando o contexto
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return new ChangePasswordResponse(true, "Password changed successfully.");
        }

        return new ChangePasswordResponse(false, "Invalid password recovery code.");
    }

    public Task<string> GetProfileImage(Guid userId)
    {
        return _userDAO.GetProfileImageUri(userId);
    }

    public Task<Guid> GetUserLikeIdBySenderAndReceiver(Guid senderUserId, Guid receiverUserId)
    {
        return _userDAO.GetUserLikeId(senderUserId, receiverUserId);
    }
}
