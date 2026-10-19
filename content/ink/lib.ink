// ─────────────────────────────────────────────────────────────
// Engine bridge. All world state lives in the engine; ink reads
// and writes it through these functions.
// ─────────────────────────────────────────────────────────────

EXTERNAL q(name)                 // value of a quality or derived stat
EXTERNAL qset(name, value)
EXTERNAL qadd(name, delta)
EXTERNAL remember(who, tag)      // someone stores a memory of Teo
EXTERNAL remembers(who, tag)
EXTERNAL regard(who)             // how someone feels about Teo, -10..10
EXTERNAL like(who, delta)
EXTERNAL check(id, stat)         // hidden check: 0 badly, 1 mixed, 2 well
EXTERNAL chance(id, percent)     // seeded coin; same answer on reload
EXTERNAL drink(units)
EXTERNAL pass_time(minutes)
EXTERNAL goto_phase(id)
EXTERNAL headline(text)
EXTERNAL played(storylet)
EXTERNAL why()                   // what decided the last check: drink, tired, bold, careful, luck_good, luck_bad, or ""
EXTERNAL shift(trait, delta, reason)   // a lasting change to a trait after a big moment: one step, once per reason

// Where Teo wakes up on Wednesday.
CONST AT_HOME = 1
CONST AT_MAGDAS = 2
CONST AT_KORNELS = 3
CONST AT_STATION = 4

// ── Readability helpers ──────────────────────────────────────

// 0 sober, 1 tipsy, 2 drunk, 3 very drunk, 4 wrecked
=== function drunk()
    ~ return q("drunk")

=== function tired()
    ~ return q("tired")

=== function mood()
    ~ return q("mood")

// Teo's traits as they are right now (his character, pushed by drink, mood, tiredness, shame).
// Each is a line: nerve + bold / - careful; warmth + open / - closed; appetite + hungry / - held back;
// honesty + blunt / - smooth. 0 is the middle; his baseline is careful, open, held back, smooth.
=== function nerve()
    ~ return q("nerve")

=== function warmth()
    ~ return q("warmth")

=== function appetite()
    ~ return q("appetite")

=== function honesty()
    ~ return q("honesty")

=== function hungover()
    ~ return q("hungover")

// Skills the hidden checks use, made from the traits.
=== function tact()
    ~ return q("tact")

=== function judgment()
    ~ return q("judgment")

=== function restraint()
    ~ return q("restraint")

// Hindsight: after an important check, one line names the cause. Each line once per life (two for drink).
=== function hindsight()
    ~ temp cause = why()
    { cause == "drink":
        ~ qadd("said_drink", 1)
        { q("said_drink") == 1:
            ~ return "That was the zhivka talking. The zhivka has a lot to say."
        }
        { q("said_drink") == 2:
            ~ return "Sober me would never. Sober me wasn't invited."
        }
        ~ return ""
    }
    { cause != "" && q("said_" + cause) == 0:
        ~ qset("said_" + cause, 1)
        { cause == "bold":
            ~ return "I said it because I'm the kind of man who says it. Apparently."
        }
        { cause == "careful":
            ~ return "I waited. I always wait. Sometimes the moment doesn't."
        }
        { cause == "tired":
            ~ return "I was too tired to be careful. Careful takes energy."
        }
        { cause == "luck_good":
            ~ return "It worked. I'd like to say I knew it would."
        }
        { cause == "luck_bad":
            ~ return "I did everything right. It went wrong anyway. That's also a Tuesday."
        }
    }
    ~ return ""

=== function is(name)
    ~ return q(name) != 0

=== function flag(name)
    ~ qset(name, 1)

=== function have_drink(units)
    ~ drink(units)
    ~ qadd("drinks", 1)
    { drunk() > q("peak_drunk"):
        ~ qset("peak_drunk", drunk())
    }

=== function humiliate(amount)
    ~ qadd("shame", amount)
    ~ qadd("mood", -1)

// ── End of the day: who Teo was ─────────────────────────────
// Up to three lines, picked by what actually happened. Shown on the end screen above the headlines.

=== today
~ temp n = 0
{ q("shifts") > 0:
    Something changed in Teo today. He'll notice next week.
    ~ n++
}
{ q("shame") >= 1:
    Teo was laughed at today. He'll laugh about it by Christmas. Maybe.
    ~ n++
}
{ q("peak_drunk") >= 3:
    Teo was braver than he is tonight. Mostly the zhivka.
    ~ n++
- else:
    { q("peak_drunk") == 0 && played("party_arrive") > 0:
        Teo stayed sober. He saw everything. Nobody thanked him.
        ~ n++
    }
}
{ n < 3 && q("chose_bold") >= 4 && q("chose_bold") > q("chose_cautious"):
    Teo said things today he usually only thinks.
    ~ n++
}
{ n < 3 && q("chose_cautious") >= 4 && q("chose_cautious") > q("chose_bold"):
    Teo waited for the right moment. It didn't come. It rarely does.
    ~ n++
}
{
- n < 3 && q("chose_kind") >= 3:
    Teo let people in today. Some of them will stay.
    ~ n++
- n < 3 && q("chose_kind") == 0:
    Teo kept everyone at arm's length. It was quiet there.
    ~ n++
}
{
- n < 3 && q("trait_honesty") >= 0:
    Teo told the truth today. Someone paid for it.
    ~ n++
- n < 3:
    Teo said what people wanted to hear. Everyone likes him. Nobody knows him.
    ~ n++
}
-> DONE

// ── New life ─────────────────────────────────────────────────
// Runs once, silently, when a game starts. All randomness is seeded.

=== init
    // Starting mood. Mostly flat; sometimes good, sometimes bad.
    {
    - chance("mood_good", 15):
        ~ qset("mood", 1)
    - chance("mood_bad", 20):
        ~ qset("mood", -2)
    - chance("mood_ok", 30):
        ~ qset("mood", 0)
    - else:
        ~ qset("mood", -1)
    }

    // Teo's traits are set by the engine before this runs: the creator's baseline, one or two nudged a step.

    // Outsiders at the party: two of three, drawn per life.
    {
    - chance("outsiders_a", 34):
        ~ flag("outsider_consultants")
        ~ flag("outsider_hen")
    - chance("outsiders_b", 50):
        ~ flag("outsider_consultants")
        ~ flag("outsider_developer")
    - else:
        ~ flag("outsider_hen")
        ~ flag("outsider_developer")
    }

    // The thing about Zdenka. Most lives never see it.
    { chance("zdenka_dark", 15):
        ~ flag("zdenka_dark")
    }

    // Starting regard.
    ~ like("magda", 2)
    ~ like("kornel", 1)
    ~ like("viktor", 1)
    ~ like("zdenka", 0)
    ~ like("stojan", 1)
    -> DONE
