using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CSC.DigitalServer.Core
{
	public static class Constants
	{
		public const string LogoutMessageType = "Logout";
		public const string DuplicateLogin = "DuplicateLogin";
		public const string PauseResumeActiveLasers = "PauseReactivateLasers";
		public const string AddRemoveInstruction = "AddRemoveInstruction";
		public const string TraceOutput = "TraceOutput";
		//public static string SELECTEDPALLET = "SELECTEDPALLET";
		public const string DeploymentStatus = "DeploymentStatus";
		public const string Tis = "tis";
		public const string BladeRevisionPrefix = "-00";
		public const string AutoFinalize = "AutoFinalize";
		/// <summary>
		/// Direct Method timeout in seconds
		/// </summary>
		public static int DirectMethodTimeOutSec { get; private set; } = 10;

		/// <summary>
		/// Deployment timeout in seconds
		/// </summary>
		public static int DeploymentTimeOutSec { get; private set; } = 180;

		/// <summary>
		/// Triton Inference server (TIS) timeout in seconds
		/// </summary>
		public static int TisTimeOutSec { get; private set; } = 40;

		/// <summary>
		/// Get the modules list for OB toolchain
		/// </summary>
		public static readonly List<string> OBToolchainModulesList = new()
		{
			EdgeModules.LaserHubModule,
			EdgeModules.LaserFeedbackModule,
			EdgeModules.DecisionMakerModule,
			EdgeModules.ImageGrabberModule,
			EdgeModules.EdgeVerificationModule,
			EdgeModules.EdgeDetectionModule,
			EdgeModules.DataProvisioning,
			EdgeModules.DataCollector,
			EdgeModules.ConfigProxy
		};

		/// <summary>
		/// Get the modules list for Calibration toolchain
		/// </summary>
		public static readonly List<string> CalibrationToolchainModulesList = new()
		{
			EdgeModules.CalibrationOrchestrator,
			EdgeModules.CameraCalibration,
			EdgeModules.MarkerDetection,
			EdgeModules.ImageGrabberModule,
			EdgeModules.LaserHubModule
		};

		public static void UpdateTimers(int deploymentTimer, int methodtimer, int tistimer)
		{
			DeploymentTimeOutSec = deploymentTimer;
			DirectMethodTimeOutSec = methodtimer;
			TisTimeOutSec = tistimer;
		}

		/// <summary>
		/// Constants for the Cosmos DB containers
		/// </summary>
		public static class CosmosDBContainer
		{
			public const string UserSession = "UserSession";
			public const string UsersData = "UsersData";
			public const string CalibrationStatus = "CalibrationStatus";
			public const string EdgeServerHostnameMapping = "EdgeServerHostnameMapping";
			public const string ProsoftMapping = "ProsoftMapping";
			public const string DeploymentPlan = "DeploymentPlan";
			public const string PalletStore = "PalletStore";
			public const string CameraMapping = "PalletStore";
			public const string CameraCalibrationState = "fedatabase";
			public const string CosmosDbName = "fedatabase";
		}

		/// <summary>
		/// Constants for the Edge Modules
		/// </summary>
		public static class EdgeModules
		{
			public const string LaserHubModule = "laser-hub";
			public const string DecisionMakerModule = "decision-maker";
			public const string LaserFeedbackModule = "laser-feedback";
			public const string EdgeVerificationModule = "edge-verification";
			public const string ImageGrabberModule = "image-grabber";
			public const string EdgeDetectionModule = "edge-detection";
			public const string CalibrationOrchestrator = "calibration-orchestrator";
			public const string CameraCalibration = "camera-calibration";
			public const string MarkerDetection = "marker-detection";
			public const string DataProvisioning = "data-provisioning";
			public const string DataCollector = "data-collector";
			public const string ConfigProxy = "config-proxy";
		}

		public static class DMInstructions
		{
			public const string AddTeamInstruction = "add_team_instruction";
			public const string RemoveTeamInstruction = "remove_team_instruction";
			public const string PauseTeamInstruction = "pause_team_instruction";
			public const string ResumeTeamInstruction = "resume_team_instruction";
		}

		[Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
		public enum DeploymentStatusType
		{
			InProgress,
			Deployed,
			NotDeployed
		}

		[Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
		public enum UserRoles
		{
			Dev,
			Admin,
			Operator,
			TeamLead
		}

		[Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
		public enum PalletStatus
		{
			Active,
			StandBy,
			Complete
		}

		[Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
		public enum HealthStatusType
		{
			//Server is healthy
			Healthy,
			//Server is unhealthy because of module error
			UnHealthyModuleError,
			//Server is unhealthy because server itself is not working
			UnHealthyServerError
		}

		[Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
		public enum DeploymentType
		{
			Empty,
			Production,
			Calibration
		}

		public static class Profile
		{
			public const string Default = "default";
			public const string Experimental = "experimental";
		}

		[Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
		public enum AutoFinalizeStatusType
		{
			SentForConfirmation,
			Cancelled,
			None
		}
	}
}
