/*
 * FILE          : MSTestSettings.cs
 * PROJECT       : CSCN72010 - LunchSplit (Assignment 3)
 * PROGRAMMER    : Kim Quynh Huong Tuong
 * FIRST VERSION : 2026-11-12
 * DESCRIPTION   : Assembly-level MSTest settings. Runs test methods in parallel
 *                 so the test suite finishes quickly.
 */

[assembly: Parallelize(Scope = ExecutionScope.MethodLevel)]

