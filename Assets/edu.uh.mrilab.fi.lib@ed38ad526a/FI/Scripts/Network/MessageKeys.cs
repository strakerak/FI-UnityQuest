// Request and Response message types
namespace fi {
    /// <summary>
    /// The type of Visuals. TODO: Rename to EVisual.
    /// </summary>
    public enum EObjectType {
        UNKNOWN,
        IMAGE_SLICE,
        STUDY_IMAGE_SLICE,
        MODEL,
        ASSEMBLY
    }

    /// <summary>
    /// The different formats an image slace can be in.
    /// </summary>
    public enum EImageSliceFormat {
        UNKNOWN,
        GRAYSCALE,
        RGB,
        RGBI
    }

    /// <summary>
    /// The different formats a model can be in.
    /// </summary>
    public enum EModelFormat {
        UNKNOWN,
        XYZ
    }

    /// <summary>
    /// The orientations a slice can be in
    /// </summary>
    public enum ESliceOrientation {
        UNKNOWN,
        XY,
        YZ,
        XZ
    }

    /// <summary>
    /// The type of a message can be.
    /// </summary>
    public enum EMessageType {
        ERROR = 0,
        AUTHENTICATION = 1,
        APPLICATION = 2,
        MODULE = 3,
        DATA = 4
    }

    /// <summary>
    /// The type of application requests there is.
    /// </summary>
    // Request message keys
    public enum EApplicationRequest {
        UNKNOWN = 0,
        START_MODULE = 1,
        STOP_MODULE = 2,
        LOG = 3
    }

    /// <summary>
    /// The type of module requests there is.
    /// </summary>
    public enum EModuleRequest {
        UNKNOWN,
        MODULE_INTERACTION,
        SUBSCRIBE_TO_MODULE,
        UNSUBSCRIBE_TO_MODULE,
        GET_SCENE,
        GET_VISUAL,
        HIDE_VISUAL,
        TRANSLATE_VISUAL,
        ROTATE_VISUAL,
        SELECT_SLICE
    }

    /// <summary>
    /// The types of data there is.
    /// </summary>
    public enum EDataType {
        UNKNOWN,
        IMAGE,
        SERIES,
        STUDY,
        MODEL,
        ANIMATED_MODEL
    }

    /// <summary>
    /// The status of a response.
    /// </summary>
    public enum EResponseStatus {
        UNKNOWN,
        SUCCESS,
        ERROR,
        INFO_REQUIRED
    }

    /// <summary>
    /// The type of module response there is.
    /// </summary>
    public enum EModuleResponse {
        UNKNOWN,
        ADD_MODULE_INTERACTION,
        UPDATE_MODULE_INTERACTION,
        REMOVE_MODULE_INTERACTION,
        ADD_VISUAL,
        REFRESH_VISUAL,
        REMOVE_VISUAL,
        DATA_CHANGE,
        HIDE_VISUAL,
        TRANSFORM_VISUAL,
        PARENT_CHANGE,
        SET_VISUAL_OPACITY,
        SET_SLICE,
        SET_COLOR
    }

    /// <summary>
    /// The type of module interactions there is.
    /// </summary>
    public enum EModuleInteraction {
        UNKNOWN,
        VALUELESS,
        BOOL,
        INTEGER,
        FLOAT,
        STRING,
        SELECT,
        POINT_2D,
        POINT_3D
    }

    /// <summary>
    /// The type of module interaction constraints there is.
    /// </summary>
    public enum EModuleInteractionConstraint {
        UNKNOWN,
        MIN,
        MAX,
        RANGE
    }
}