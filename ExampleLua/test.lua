
function HSV(h, s, v)
    if s <= 0 then return v,v,v end
    h = h*6
    local c = v*s
    local x = (1-math.abs((h%2)-1))*c
    local m,r,g,b = (v-c), 0, 0, 0
    if h < 1 then
        r, g, b = c, x, 0
    elseif h < 2 then
        r, g, b = x, c, 0
    elseif h < 3 then
        r, g, b = 0, c, x
    elseif h < 4 then
        r, g, b = 0, x, c
    elseif h < 5 then
        r, g, b = x, 0, c
    else
        r, g, b = c, 0, x
    end
    return r+m, g+m, b+m
end

function mod( f )
    return f - math.floor(f)
end


author_pos = nil
custom_music_label = nil
custom_music_author = nil

heart_trans = nil
heart_sprite = nil

track_name = nil

h = 0
function on_frame()
    log_osd("test osd message counter", 2.0)
    --track_name.set_text("As easy as this")
    --x = author_pos.x + math.sin(ctx.current_beat) * 64
    --custom_music_author.set_position( 0, author_pos.y, author_pos.z )
    true_beat_number.set_text("Song 89/101 ("..tostring(math.floor(ctx.current_beat))..")")
    --heart_trans.set_anchor_position(600, -300)

    if ctx.in_vibe then
        r, g, b = HSV( mod(h),1,1)
        heart_sprite.set_color(r,g,b,1)
        h = h + ctx.delta_time * 0.2
    else
        heart_sprite.set_color(1,1,0,1)
        h = 50/360
    end
end

function on_beat( beat )
    custom_music_label.set_text("SAVE the World")
    r, g, b = HSV( mod(ctx.current_beat/16),1,1)
    hex = string.format("#%02X%02X%02X", r*255, g*255, b*255 )
    custom_music_author.set_text("Charted by: <"..hex..">Katie</color>")
end

function full_rect( transform )
    transform.set_anchor_max(1,1)
    transform.set_anchor_min(0,0)
    transform.set_size_delta(0,0)
    transform.set_offset_max(0,0)
    transform.set_offset_min(0,0)
end

function post()
    ctx.get_image("RhythmRiftCanvas/ScreenContainer/AccuracyBar/RatingBandsParent/GreatBand").set_color(1,1,1,1)
    ctx.get_image("RhythmRiftCanvas/ScreenContainer/AccuracyBar/RatingBandsParent/PerfectBand").set_color(1,1,1,1)

    --ctx.list_all_children()
end

function Init()
    --load assets
    load_texture("heart", "heart.png", 100, 0.5, 0.5)
    load_texture("heart-flash", "heart_flash.png", 100, 0.5, 0.5)
    load_texture("heart-heal", "heart_heal.png", 100, 0.5, 0.5)
    load_texture("heart-outline", "heart_outline.png", 100, 0.5, 0.5)
    load_texture("heart-purple", "heart_purple.png", 100, 0.5, 0.5)
    load_texture("heart-white", "heart_white.png", 100, 0.5, 0.5)
    load_texture("empty", "none.png", 100, 0.5, 0.5)
    load_texture("knife", "knife.png", 100, 0.5, 0.5)
    load_texture("test", "test.png", 100, 0.5, 0.5)
    load_texture("gband", "great_band.png", 100, 0.5, 0.5)
    load_texture("pband", "perfect_band.png", 100, 0.5, 0.5)

    --enable hooks
    log_osd("test osd messages from lua fades after 5s", 5.0)
    err_osd("test error messages from lua fades after 10s", 10.0)

    log("testing hook system")
    ctx.on_frame.add(on_frame)
    ctx.on_beat.add(on_beat)
    ctx.on_post_init.add(post)

    --find paths to objects
    --ctx.list_all_children()
    --ctx.list_all_components("RhythmRiftCanvas/ScreenContainer/AccuracyBar/RatingBandsParent/GreatBand")
    
    --cache references to relevant objects
    custom_music_label = ctx.get_tmpro("RhythmRiftCanvas/ScreenContainer/TopRightView/CustomMusicTextView")
    custom_music_author = ctx.get_tmpro("RhythmRiftCanvas/ScreenContainer/TopRightView/CustomMusicCreatorTextView")
    track_name = ctx.get_tmpro("RhythmRiftCanvas/ScreenContainer/RhythmRiftBattleUI/RRTrackUI/TrackName")
    true_beat_number = ctx.get_tmpro("RhythmRiftCanvas/ScreenContainer/TrueBeatNumberText")


    --vibe power
    ctx.get_transform("RhythmRiftCanvas/ScreenContainer/RhythmRiftBattleUI/Content/PlayerHealth/VibePowerMeter/VibePowerBoltMin").set_active( false )
    ctx.get_transform("RhythmRiftCanvas/ScreenContainer/RhythmRiftBattleUI/Content/PlayerHealth/VibePowerMeter/VibePowerBoltMax").set_active( false )

    --health bar
    ctx.get_image("RhythmRiftCanvas/ScreenContainer/RhythmRiftBattleUI/Content/PlayerHealth/VibePowerMeter/Guitar").set_sprite("knife")

    heart_trans = ctx.get_transform("RhythmRiftCanvas/ScreenContainer/RhythmRiftBattleUI/Content/PlayerHealth/PlayerHealthBar_Group/PlayerHealthBar")
    heart_sprite = heart_trans.get_image()

    heart_heal = ctx.get_image("RhythmRiftCanvas/ScreenContainer/RhythmRiftBattleUI/Content/PlayerHealth/PlayerHealthBar_Group/PlayerHealthBar_Healing")
    heart_flash = ctx.get_image("RhythmRiftCanvas/ScreenContainer/RhythmRiftBattleUI/Content/PlayerHealth/PlayerHealthBar_Group/PlayerHealthBar-Flash")
    heart_outline = ctx.get_image("RhythmRiftCanvas/ScreenContainer/RhythmRiftBattleUI/Content/PlayerHealth/PlayerHealthBar_Group/PlayerHealthBar_Border")
    heart_bg = ctx.get_image("RhythmRiftCanvas/ScreenContainer/RhythmRiftBattleUI/Content/PlayerHealth/PlayerHealthBar_Group/PlayerHealthBar_BG")
    heart_back = ctx.get_image("RhythmRiftCanvas/ScreenContainer/RhythmRiftBattleUI/Content/PlayerHealth/PlayerHealth-HeartBacker")

    heart_sprite.set_sprite("heart-white")
    heart_sprite.set_color(1,1,1,1)
    heart_heal.set_sprite("heart-heal")
    heart_flash.set_sprite("heart-white")
    heart_bg.set_sprite("heart")
    heart_outline.set_sprite("heart-outline")
    heart_back.set_sprite("empty")
    ctx.get_image("RhythmRiftCanvas/ScreenContainer/RhythmRiftBattleUI/Content/PlayerHealth/PlayerHealthBar_Group/PlayerHealthBar_Healed").set_sprite("empty")    

    full_rect(heart_trans)
    full_rect(heart_outline.get_transform())
    full_rect(heart_heal.get_transform())
    full_rect(heart_bg.get_transform())
    full_rect(heart_flash.get_transform())

    --rot = heart_trans.get_rotation()
    heart_trans.set_rotation(40,0,0)
    heart_outline.get_transform().set_rotation(40,0,0)
    heart_heal.get_transform().set_rotation(40,0,0)
    heart_flash.get_transform().set_rotation(40,0,0)
    heart_bg.get_transform().set_rotation(40,0,0)

    heart_trans.set_anchor_position(-32,-235)
    heart_outline.get_transform().set_anchor_position(-32,-235)
    heart_heal.get_transform().set_anchor_position(-32,-235)
    heart_flash.get_transform().set_anchor_position(-32,-235)
    heart_bg.get_transform().set_anchor_position(-32,-235)

    --acc bar
    ctx.get_image("RhythmRiftCanvas/ScreenContainer/AccuracyBar/RatingBandsParent/GreatBand").set_sprite("gband")
    ctx.get_image("RhythmRiftCanvas/ScreenContainer/AccuracyBar/RatingBandsParent/PerfectBand").set_sprite("pband")


    --testing adding text
    lv_back = ctx.get_transform("RhythmRiftCanvas/ScreenContainer/").add_child("LV Backer")
    --lv_back = ctx.get_transform("RhythmRiftCanvas/ScreenContainer/RhythmRiftBattleUI/Content/PlayerScore/ComboBacker").add_child("LV Backer")

    --lv_text_con = lv_back.add_child("LV TEXT")
    --lv_text = lv_text_con.add_tmpro()
    --lv_text.set_text("test")

    --lv_img_con = lv_back.add_child("LV IMG")
    --lv_img = lv_img_con.add_image("heart")

    --ctx.get_tmpro("RhythmRiftCanvas/ScreenContainer/LuaOSD/LuaOSDText").set_text("test\ntest\ntest\ntesttestesttest")
    --ctx.list_all_children()
end