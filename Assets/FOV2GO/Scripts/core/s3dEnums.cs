using System.Collections;

/* This file is part of Stereoskopix FOV2GO for Unity V3.
 * URL: http://www.stereoskopix.com/ * Please direct any bugs/comments/suggestions to hoberman@usc.edu.
 * Stereoskopix FOV2GO for Unity Copyright (c) 2011-12 Perry Hoberman & MxR Lab. All rights reserved.
 *
 * s3d Enums Script revised 12.30.12
 * Contains all enums used in Stereoskopix FOV2GO package 
 */
public enum cams3D
{
    Left_Right = 0,
    Left_Only = 1,
    Right_Only = 2,
    Right_Left = 3
}

public enum mode3D
{
    SideBySide = 0,
    Anaglyph = 1,
    OverUnder = 2,
    Interlace = 3,
    Checkerboard = 4
}

public enum anaType
{
    Monochrome = 0,
    HalfColor = 1,
    FullColor = 2,
    Optimized = 3,
    Purple = 4
}

public enum phoneType
{
    GalaxyNexus_LandLeft = 0,
    GalaxyNote_LandLeft = 1,
    iPad2_LandLeft = 2,
    iPad2_Portrait = 3,
    iPad3_LandLeft = 4,
    iPad3_Portrait = 5,
    iPhone4_LandLeft = 6,
    OneS_LandLeft = 7,
    Rezound_LandLeft = 8,
    Thrill_LandLeft = 9,
    my3D_LandLeft = 10
}

public enum maskDistance
{
    MaxDistance = 0,
    ScreenPlane = 1,
    FarFrustum = 2
}

public enum rayCastFilter
{
    None = 0,
    Tag = 1,
    Layer = 2
}

public enum Axes
{
    MouseXandY = 0,
    MouseX = 1,
    MouseY = 2
}

public enum controlGui
{
    none = 0,
    move = 1,
    turn = 2,
    point = 3
}

public enum controlPos
{
    off = 0,
    left = 1,
    center = 2,
    right = 3
}

public enum converge
{
    none = 0,
    percent = 1,
    center = 2,
    click = 3,
    mouse = 4,
    @object = 5
}

public enum sPlane
{
    near = 0,
    screen = 1,
    far = 2
}