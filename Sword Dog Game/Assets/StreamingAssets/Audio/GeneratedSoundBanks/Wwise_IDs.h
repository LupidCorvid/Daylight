/////////////////////////////////////////////////////////////////////////////////////////////////////
//
// Audiokinetic Wwise generated include file. Do not edit.
//
/////////////////////////////////////////////////////////////////////////////////////////////////////

#ifndef __WWISE_IDS_H__
#define __WWISE_IDS_H__

#include <AK/SoundEngine/Common/AkTypes.h>

namespace AK
{
    namespace EVENTS
    {
        static const AkUniqueID DEFEATBOSS = 2214005863U;
        static const AkUniqueID DIE = 445985469U;
        static const AkUniqueID ENDNPC = 3630201441U;
        static const AkUniqueID ENTERAMBIENCE = 1656162925U;
        static const AkUniqueID ENTERBLACKSMITH = 3862063455U;
        static const AkUniqueID ENTERCAVES = 1967729093U;
        static const AkUniqueID ENTERDESERTCAVES = 2331186804U;
        static const AkUniqueID ENTERDOJO = 2407129755U;
        static const AkUniqueID ENTERFOREST = 2024283954U;
        static const AkUniqueID ENTERHOUSE = 3190755319U;
        static const AkUniqueID ENTERINTERIOR = 176800609U;
        static const AkUniqueID ENTERMEDIC = 3549296221U;
        static const AkUniqueID ENTERMEDICINSTANT = 3816747828U;
        static const AkUniqueID ENTERMOUNTAINCAVES = 1067397042U;
        static const AkUniqueID ENTERMOUNTAINS = 2342301127U;
        static const AkUniqueID ENTEROCEAN = 3252024581U;
        static const AkUniqueID ENTERSHOP = 1104863757U;
        static const AkUniqueID ENTERTOWN = 1143074605U;
        static const AkUniqueID ENTERTUTORIAL = 1763353983U;
        static const AkUniqueID EXITAMBIENCE = 2442109625U;
        static const AkUniqueID EXITBLACKSMITH = 2009297027U;
        static const AkUniqueID EXITBLACKSMITHDOJO = 1790250137U;
        static const AkUniqueID EXITDOJO = 3270207375U;
        static const AkUniqueID EXITHOUSE = 3795427123U;
        static const AkUniqueID EXITINTERIOR = 3488912141U;
        static const AkUniqueID EXITMEDIC = 3217358681U;
        static const AkUniqueID FADEOUTALL = 3208992266U;
        static const AkUniqueID FIGHTBOSS = 2874785730U;
        static const AkUniqueID FIRSTTALKRICKEN = 3096686399U;
        static const AkUniqueID HURT = 3193947170U;
        static const AkUniqueID LEAVEAREA = 1553760757U;
        static const AkUniqueID MONSTERSAWARE = 2526784182U;
        static const AkUniqueID MONSTERSUNAWARE = 3272068613U;
        static const AkUniqueID NEXTPHASE = 513969863U;
        static const AkUniqueID PAUSE = 3092587493U;
        static const AkUniqueID PLAYAREA = 1670305508U;
        static const AkUniqueID PLAYCREDITS = 2250688815U;
        static const AkUniqueID PLAYINTRO = 781908707U;
        static const AkUniqueID PLAYMENU = 1862289782U;
        static const AkUniqueID PLAYNPC = 2188110408U;
        static const AkUniqueID RESUME = 953277036U;
        static const AkUniqueID SECONDTALKRICKEN = 2104656083U;
        static const AkUniqueID TALKGENERAL = 3087694289U;
    } // namespace EVENTS

    namespace STATES
    {
        namespace AREA
        {
            static const AkUniqueID GROUP = 3686139462U;

            namespace STATE
            {
                static const AkUniqueID CAVES = 749373321U;
                static const AkUniqueID DESERT = 1850388778U;
                static const AkUniqueID DESERTCAVES = 3047821848U;
                static const AkUniqueID FOREST = 491961918U;
                static const AkUniqueID MOUNTAINCAVES = 2777163414U;
                static const AkUniqueID MOUNTAINS = 3992294315U;
                static const AkUniqueID NONE = 748895195U;
                static const AkUniqueID OCEAN = 3802555985U;
                static const AkUniqueID PROLOGUE = 4203473902U;
                static const AkUniqueID TOWN = 3091570009U;
            } // namespace STATE
        } // namespace AREA

        namespace NPC
        {
            static const AkUniqueID GROUP = 662417162U;

            namespace STATE
            {
                static const AkUniqueID GENERAL = 133642231U;
                static const AkUniqueID NONE = 748895195U;
                static const AkUniqueID RICKEN = 4021494753U;
            } // namespace STATE
        } // namespace NPC

    } // namespace STATES

    namespace SWITCHES
    {
        namespace FOREST
        {
            static const AkUniqueID GROUP = 491961918U;

            namespace SWITCH
            {
                static const AkUniqueID COMBAT = 2764240573U;
                static const AkUniqueID EXPLORATION = 2582085496U;
            } // namespace SWITCH
        } // namespace FOREST

        namespace PROLOGUE
        {
            static const AkUniqueID GROUP = 4203473902U;

            namespace SWITCH
            {
                static const AkUniqueID BOSS = 1560169506U;
                static const AkUniqueID TUTORIAL = 3762955427U;
            } // namespace SWITCH
        } // namespace PROLOGUE

    } // namespace SWITCHES

    namespace GAME_PARAMETERS
    {
        static const AkUniqueID AMBIENCE = 85412153U;
        static const AkUniqueID ENEMYAWARE = 3963624107U;
        static const AkUniqueID HEALTH = 3677180323U;
        static const AkUniqueID HEIGHT = 1279776192U;
        static const AkUniqueID INTERIOR = 1132214669U;
        static const AkUniqueID MUSICVOLUME = 2346531308U;
        static const AkUniqueID PROLOGUEPHASE = 2827251531U;
        static const AkUniqueID SFXVOLUME = 988953028U;
    } // namespace GAME_PARAMETERS

    namespace BANKS
    {
        static const AkUniqueID INIT = 1355168291U;
        static const AkUniqueID MAIN = 3161908922U;
    } // namespace BANKS

    namespace BUSSES
    {
        static const AkUniqueID GLOBALSFX = 2883715697U;
        static const AkUniqueID MASTER_AUDIO_BUS = 3803692087U;
        static const AkUniqueID MUSIC = 3991942870U;
        static const AkUniqueID SFX = 393239870U;
    } // namespace BUSSES

    namespace AUDIO_DEVICES
    {
        static const AkUniqueID NO_OUTPUT = 2317455096U;
        static const AkUniqueID SYSTEM = 3859886410U;
    } // namespace AUDIO_DEVICES

}// namespace AK

#endif // __WWISE_IDS_H__
