// ═════════════════════════════════════════════════════════════
// TUESDAY · EVENING · What everyone else is doing
// Each person has a plan for the evening and follows it whether Teo is there or not
// (design/agendas_build.md). Offstage knots only change the world; Teo catches the odd
// overheard line. At most one person comes looking for Teo per evening (came_to_teo).
// ═════════════════════════════════════════════════════════════

// ── Magda: get Viktor tonight ────────────────────────────────

=== magda_drinks
# phase: party
# kind: offstage
# when: open_bar and doing(magda, door)
~ feel("magda", 1)
~ flag("magda_drinking")
MAGDA: Laci! I'm the bank! The bank says yes! #heard:by the door #from:left
-> DONE


// An hour after Viktor turned away, if nobody came down for her, she comes back up.
=== magda_comes_back
# phase: party
# kind: offstage
# when: doing(magda, river) and not magda_walk_offer and time >= magda_went_down + 60
~ feel("magda", 1)
{ is("outsider_hen"):
    ~ set_doing("magda", "hen")
    DENISA: Girls! She's one of us now! #heard:from the hen party #from:right
- else:
    ~ set_doing("magda", "bar")
    MAGDA: Laci. The bank is open again. #heard:at the bar #from:left
}
-> DONE


=== magda_back
# phase: party
# where: magda
# kind: hub
# choice: Magda is back.
# when: doing(magda, bar) or doing(magda, hen)
# duration: 20
{ doing("magda", "hen"):
    The hen party has her now. Someone has put a veil on her, and the bride doesn't seem to mind.
    MAGDA: Teo! These are my friends! I met them twenty minutes ago!
- else:
    She's on a stool at the bar with her shoes off and her lipstick back on, slightly to the left of where her mouth is.
    MAGDA: Don't ask. I'm fine. I'm having a lovely time. Laci, tell him.
    LACI: She's having a lovely time.
}
* {warmth() >= 0} [Sit with her a while. #kind]
    ~ like("magda", 2)
    ~ remember("magda", "sat_with_later")
    ~ flag("magda_walk_offer")
    We don't talk about Viktor. We talk about the tram, and her son's teacher, and whether pigeons have funerals. She thinks yes.
    MAGDA: Walk me home later? I don't want the tram looking like this.
* [Get her a water.]
    ~ like("magda", 1)
    This time I actually get the water. She drinks half and uses the rest to clean her face in the mirror behind the bar.
    MAGDA: Better?
    It isn't. I say it is. That's what the water was for.
* [Leave her to it.]
    MAGDA: Go on. Go. I'm fine. I said.
- -> DONE


=== magda_finds_teo
# phase: party
# kind: interrupt
# at: 22:00
# priority: 22
# when: (doing(magda, bar) or doing(magda, hen)) and regard(magda) >= 2 and not magda_walk_offer and not came_to_teo
~ flag("came_to_teo")
Magda finds me. Shoes in one hand, bag in the other, like a woman leaving a burning building in no particular hurry.
MAGDA: Teo. Walk me home? I don't want the tram looking like this.
* [Walk her home. #kind]
    ~ like("magda", 1)
    TEO: Now?
    MAGDA: Now. Before I say goodbye to everyone. That takes an hour, with me.
    ~ set_doing("magda", "gone")
    ~ goto_phase("night_magda")
* [Not tonight, Magda.]
    ~ like("magda", -1)
    ~ remember("magda", "said_no_walk")
    MAGDA: No. Course. You're right. I'm a grown woman. I've got a phone. I've got an app.
    She hasn't got the app.
- -> DONE


// Nobody is taking her home, so she goes with someone. Who it is, is luck.
=== magda_leaves_with_someone
# phase: party
# kind: offstage
# at: 22:00
# chance: 40
# when: (doing(magda, bar) or doing(magda, hen)) and not magda_walk_offer and (regard(magda) < 2 or time >= 22:30)
~ set_doing("magda", "gone")
~ flag("magda_left_with_someone")
~ feel("magda", 1)
{
- is("outsider_consultants") && chance("magda_with_maarten", 50):
    ~ flag("magda_left_with_maarten")
    ~ headline("Magda left the party with one of the consultants.")
    MAARTEN: I'll get you a taxi. I'll come with you. To make sure you get home. #heard:by the way out #from:left
- is("outsider_developer") && chance("magda_with_adrian", 50):
    ~ flag("magda_left_with_adrian")
    ~ headline("Magda left the party in Adrian Kovál's car.")
    ADRIAN: My driver's outside. He goes wherever you like. #heard:by the way out #from:left
- else:
    ~ flag("magda_left_with_stranger")
    ~ headline("Magda left the party with a man nobody knew.")
    MAN: I know a place. It's open late. Everything's open late if you know a place. #heard:by the way out #from:left
}
-> DONE


// ── Viktor: tonight's target, then the club ──────────────────

// He leaves at half nine, or waits until half ten for a Teo he has invited.
=== viktor_to_club
# phase: party
# kind: offstage
# at: 21:30
# when: doing(viktor, railing) and not viktor_foiled and (not viktor_invite or time >= 22:30)
~ set_doing("viktor", "club")
~ feel("viktor", 2)
VIKTOR: Schengen! Who's young? Nobody? Fine. Come anyway. #heard:by the railing #from:right
-> DONE


// Ten o'clock, if Teo ruined his evening: half the time he comes for Teo, half the time he drinks on alone.
=== viktor_decides
# phase: party
# kind: offstage
# at: 22:00
# when: doing(viktor, bar) and not viktor_decided
~ flag("viktor_decided")
{ not is("came_to_teo") && chance("viktor_comes_for_teo", 50):
    ~ flag("viktor_will_ask")
}
-> DONE


=== viktor_takes_teo
# phase: party
# kind: interrupt
# priority: 23
# when: viktor_will_ask and doing(viktor, bar) and not came_to_teo
~ flag("came_to_teo")
Viktor has been at the bar for an hour, drinking alone. I've never seen him do anything alone. He has people for it.
He puts a hand on the back of my neck. Not hard. Just so I know it's there.
VIKTOR: You. You're coming. You owe me an evening. #mood:displeased
* [Go with him.]
    ~ like("viktor", 1)
    ~ remember("viktor", "came_anyway")
    ~ set_doing("viktor", "club")
    I go with him. Of course I go with him. I ruined his evening; the least I can do is watch him fix it.
    ~ goto_phase("night_club")
* [Not tonight, Viktor.]
    ~ like("viktor", -1)
    ~ remember("viktor", "refused_club")
    VIKTOR: No. Course not. You've had a big night. You've been very busy.
    He goes on his own. The car door shuts like a full stop.
    ~ set_doing("viktor", "club")
- -> DONE


=== viktor_leaves_alone
# phase: party
# kind: offstage
# at: 22:45
# when: doing(viktor, bar) and viktor_decided and (not viktor_will_ask or came_to_teo)
~ set_doing("viktor", "club")
VIKTOR: Laci. Call me a car. No. Call me two. I'll take the bigger one. #heard:at the bar #from:left
-> DONE


// ── Ramona: be seen not caring, then leave with dignity ──────

=== ramona_sees_viktor_go
# phase: party
# kind: offstage
# when: doing(viktor, club) and doing(ramona, bar) and not ramona_saw_viktor_go
~ flag("ramona_saw_viktor_go")
~ feel("ramona", -2)
RAMONA: The bottle, please. Not a glass. I'm not a child. #heard:at the bar #from:left
-> DONE


// Half ten: she goes. If her evening has gone badly, sometimes she takes Teo with her.
=== ramona_leaves
# phase: party
# kind: offstage
# at: 22:30
# when: doing(ramona, bar) and not ramona_offer_ride and (not ramona_will_ask or came_to_teo)
{ feels("ramona") <= -2 && not is("came_to_teo") && chance("ramona_lift", 50):
    ~ flag("ramona_will_ask")
- else:
    ~ set_doing("ramona", "gone")
    RAMONA: Bogdan. We're going. No, I'm not saying goodbye. They'll work it out. #heard:at the bar #from:left
}
-> DONE


=== ramona_finds_teo
# phase: party
# kind: interrupt
# priority: 24
# when: ramona_will_ask and doing(ramona, bar) and not came_to_teo
~ flag("came_to_teo")
~ set_doing("ramona", "gone")
Ramona stops next to me on her way out. Sunglasses on again, at night. The bag on her arm like a small dog that bites.
RAMONA: You. Permits. I'm leaving. You look like a man who needs a lift.
* [Take the lift.]
    ~ like("ramona", 1)
    ~ remember("ramona", "took_lift")
    I follow her to the car. Viktor isn't here to see it. I can't tell if that's a relief or a waste.
    ~ goto_phase("night_ramona")
* [I'm fine, thanks.]
    RAMONA: Nobody's fine, darling. Some of us just have a driver.
- -> DONE


// ── Kornel: be liked ─────────────────────────────────────────

=== kornel_with_zdenka
# phase: party
# kind: offstage
# at: 20:45
# when: played(kornel_poem) > 0 and kornel_poem == 0 and not stood_with_kornel and not doing(zdenka, gone)
~ set_doing("kornel", "with_zdenka")
~ feel("kornel", 1)
~ flag("zdenka_said_it_rhymed")
ZDENKA: It rhymed, Kornel. That's more than most of them manage. #heard:at Zdenka's table #from:right
-> DONE


=== kornel_first_beer
# phase: party
# kind: offstage
# at: 21:00
# when: kornel_poem == 2
~ feel("kornel", 2)
~ flag("kornel_first_beer")
{ is("outsider_hen"):
    DENISA: A beer for the poet! #heard:from the hen party #from:right
}
KORNEL: It tastes like bread that's given up! #heard #from:right
-> DONE


// ── Zdenka: see everyone together, leave at ten ──────────────

=== zdenka_leaves
# phase: party
# kind: offstage
# at: 22:00
# when: not doing(zdenka, gone)
~ set_doing("zdenka", "gone")
{ is("zdenka_dark") && not is("saw_zdenka_napkin"):
    ~ flag("zdenka_left_napkin")
}
-> DONE


=== zdenka_empty_table
# phase: party
# where: zdenka_table
# kind: hub
# choice: Zdenka's table, empty.
# when: doing(zdenka, gone)
# duration: 5
Zdenka's gone. The chair is pushed in, square to the table. Her glass is gone too. She took her glass.
{ is("zdenka_left_napkin"):
    There's a napkin in the middle of the cloth, folded in half, and in half, and in half again, into a hard little square. Exactly in the middle. Like a full stop.
    ~ flag("found_zdenka_napkin")
}
-> DONE


// ── Stojan: get the balcony through, one way or another ──────
// Teo isn't home, so none of this is seen. It's heard later, through the wall.

=== stojan_evening
# phase: party
# kind: offstage
# at: 19:00
{
- is("stojan_refused"):
    ~ set_doing("stojan", "building")
- is("stojan_promise"):
    ~ set_doing("stojan", "celebrating")
}
-> DONE
