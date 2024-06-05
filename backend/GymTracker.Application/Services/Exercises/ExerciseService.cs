using GymTracker.Application.Services.Contracts;
using GymTracker.Domain.Entities;
using GymTracker.Domain.Repositories;
using GymTracker.Infra.Data.DAOs.DefaultExercise;
using GymTracker.Infra.Data.DAOs.Exercise;
using GymTracker.Infra.Data.UnityOfWork;

namespace GymTracker.Application.Services;

public class ExerciseService : IExerciseService
{
    private readonly IExerciseRepository _exerciseRepository;
    private readonly IUnityOfWork _unityOfWork;

    private readonly IExerciseDAO _exerciseDAO;
    private readonly IDefaultExerciseDAO _defaultExerciseDAO;

    public ExerciseService(IExerciseRepository exerciseRepository, IUnityOfWork unityOfWork, IExerciseDAO exerciseDAO, IDefaultExerciseDAO defaultExerciseDAO)
    {
        _exerciseRepository = exerciseRepository;

        _unityOfWork = unityOfWork;

        _exerciseDAO = exerciseDAO;
        _defaultExerciseDAO = defaultExerciseDAO;
    }

    public async Task<RegisterMuscleGroupResponse> RegisterMuscleGroup(RegisterMuscleGroupRequest request)
    {
        try
        {
            var muscleGroup = new MuscleGroup(
                groupName: request.groupName,
                muscleImage: request.muscleImage!
            );

            await _exerciseRepository.RegisterMuscleGroup(muscleGroup);
            await _unityOfWork.Commit();

            var response = new RegisterMuscleGroupResponse(
                muscleGroupId: muscleGroup.MucleGroupId,
                groupName: muscleGroup.GroupName!,
                muscleImage: muscleGroup.MuscleImage!
            );

            return response;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<RegisterExerciseResponse> RegisterExercise(RegisterExerciseRequest request)
    {
        try
        {
            var exercise = new Exercise(
                exerciseName: request.exerciseName,
                exerciseGif: request.exerciseGif!
            );

            await _exerciseRepository.RegisterExercise(exercise);

            foreach (var muscleGroupId in request.relatedMuscleGroupIds)
            {
                var exerciseMuscleGroup = new ExerciseMuscleGroup(
                    muscleGroupId: muscleGroupId,
                    exerciseId: exercise.ExerciseId
                );

                await _exerciseRepository.LinkExerciseAndMuscleGroup(exerciseMuscleGroup);
            }

            await _unityOfWork.Commit();

            var response = new RegisterExerciseResponse(
                exerciseId: exercise.ExerciseId,
                exerciseName: exercise.ExerciseName!,
                exerciseGif: exercise.ExerciseGif
            );

            return response;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<IEnumerable<ExerciseDTO>> ListExercisesByMuscleGroupId(Guid muscleGroupId)
    {
        try
        {
            return await _exerciseDAO.ListExercisesByMuscleGroupId(muscleGroupId);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public Task<List<MuscleGroup>> ListAllMuscleGroups()
    {
        try
        {
            return _exerciseRepository.ListAllMuscleGroups();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public Task<IEnumerable<DefaultWorkoutExerciseDTO>> ListExercisesByDefaultWorkoutId(Guid defaultWorkoutId)
    {
        try
        {
            return _defaultExerciseDAO.ListExercisesByDefaultWorkout(defaultWorkoutId);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<RegisterDiaryExerciseSerieResponse> RegisterDiaryExerciseSerie(RegisterDiaryExerciseSerieRequest request)
    {
        try
        {
            var exerciseRegistry = new DiaryExerciseSerie(
                serieNumber: request.serieNumber,
                repetitions: request.repetitions,
                overload: request.overload,
                diaryExerciseId: request.diaryExerciseId
            );

            await _exerciseRepository.RegisterDiaryExerciseSerie(exerciseRegistry);

            await _unityOfWork.Commit();

            var response = new RegisterDiaryExerciseSerieResponse(
                diaryExerciseSerieId: exerciseRegistry.DiaryExerciseSerieId,
                serieNumber: exerciseRegistry.SerieNumber,
                repetitions: exerciseRegistry.Repetitions,
                overload: exerciseRegistry.Overload
            );

            return response;
        }
        catch (Exception)
        {
            throw;
        }
    }
}
