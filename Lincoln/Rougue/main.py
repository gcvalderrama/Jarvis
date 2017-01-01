from pythonrouge import pythonrouge

ROUGE = './RELEASE-1.5.5/ROUGE-1.5.5.pl'
data_path = './RELEASE-1.5.5/data'

peer = "Tokyo is the one of the biggest city in the world."
model = "The capital of Japan, Tokyo, is the center of Japanese economy."
score = pythonrouge.pythonrouge(peer, model, ROUGE, data_path)
print(score)

peer = "Tokyo is the one of the biggest city in the world."
model = "Tokyo is one of the biggest city in the world."
score = pythonrouge.pythonrouge(peer, model, ROUGE, data_path)
print(score)

peer = 'President Bush''s nomination of black conservative Clarence Thomas to ' \
       'replace the Supreme Court''s first black Justice, liberal Thurgood' \
       'Marshall, split the Senate down the middle. Thomas''s opposition to' \
       'affirmative action alienated civil rights activists while his Catholic' \
       'upbringing and interest in the priesthood raised alarm in' \
       'abortion-rights groups.  The Judiciary Committee deadlocked 7-7 and' \
       'the nomination was referred to the Senate without recommendation after' \
       'extended televised hearings on charges of sexual harassment against' \
       'the nominee. Thomas was confirmed by a close 52-48 vote but he ' \
       'commented that nothing could give him back his good name.'

model = "Clarence Thomas was confirmed as Supreme Court Justice in October 1991" \
        "by a razor- thin margin of 52-48. Thomas, who has opposed affirmative" \
        "action has not taken public stands on other key issues. His reputation" \
        "was damaged by accusations of sexual harassment. As the youngest" \
        "justice he is expected to be on the court for decades."

score = pythonrouge.pythonrouge(peer, model, ROUGE, data_path)
print(score)