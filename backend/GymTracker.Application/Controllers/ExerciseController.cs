using GymTracker.Application.Services;
using GymTracker.Application.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace GymTracker.Application.Controllers;

[Route("api/exercises")]
[ApiController]
[Produces("application/json")]
public class ExerciseController : ControllerBase
{
    private readonly IExerciseService _exerciseService;

    public ExerciseController(IExerciseService exerciseService)
    {
        _exerciseService = exerciseService;
    }

    [HttpPost("muscle_groups")]
    public async Task<IActionResult> RegisterMuscleGroup([FromBody] RegisterMuscleGroupRequest request)
    {
        try
        {
            var response = await _exerciseService.RegisterMuscleGroup(request);

            return StatusCode(201, response);
        }
        catch (Exception error)
        {
            return BadRequest(error.Message);
        }
    }

    [HttpPost("exercises")]
    public async Task<IActionResult> RegisterExercise([FromBody] RegisterExerciseRequest request)
    {
        try
        {
            var response = await _exerciseService.RegisterExercise(request);

            return StatusCode(201, response);
        }
        catch (Exception error)
        {
            return BadRequest(error.Message);
        }
    }

    [HttpPost("diary_exercise_series")]
    public async Task<IActionResult> RegisterDiaryExerciseSerie([FromBody] RegisterDiaryExerciseSerieRequest request)
    {
        try
        {
            var response = await _exerciseService.RegisterDiaryExerciseSerie(request);

            return StatusCode(201, response);
        }
        catch (Exception error)
        {
            return BadRequest(error.Message);
        }
    }

    [HttpGet("exercises")]
    public async Task<IActionResult> ListExercisesBySpecificMuscleGroup(Guid muscleGroupId)
    {
        try
        {
            var response = await _exerciseService.ListExercisesByMuscleGroupId(muscleGroupId);

            return Ok(response);
        }
        catch (Exception error)
        {
            return BadRequest(error.Message);
        }
    }

    [HttpGet("/api/muscle_groups")]
    public async Task<IActionResult> ListAllMuscleGroups()
    {
        try
        {
            var response = await _exerciseService.ListAllMuscleGroups();

            return Ok(response);
        }
        catch (Exception error)
        {
            return BadRequest(error.Message);
        }
    }
}
