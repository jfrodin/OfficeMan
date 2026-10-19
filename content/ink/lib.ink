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
