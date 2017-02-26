library(readxl)
library(nortest)

rouge_conceptual <- read_excel("D:/Tesis2016/Jarvis/Final/rouge_test_final.xlsx", 
                               sheet = "ConceptualRST")
rouge_no_expantion_conceptual <- read_excel("D:/Tesis2016/Jarvis/Final/rouge_test_final.xlsx", 
                               sheet = "NLG")
#View(rouge_final_conceptual)
#View(rouge_conceptual)

first_vector <- rouge_conceptual[,6]
second_vector <- rouge_no_expantion_conceptual[,6]


print(lillie.test(first_vector))
print(lillie.test(second_vector))

#wilconxon = wilcox.test(first_vector, second_vector)
#print(wilconxon)

ttest = t.test(first_vector, second_vector, mu=0, alternative = "two.sided", paired = T, conf.level = 0.95)
print(ttest)
#boxplot(first_vector, second_vector)
#plot(first_vector, second_vector)
#abline(a=0,b=1)
#lot(density(first_vector))
#hist(first_vector)
#qqnorm(second_vector)


print(round(mean(first_vector),3))
print(round(mean(second_vector),3))

#boxplot(first_vector, second_vector)


#qqnorm(rouge1_rst_noexp)
#qqline(rouge1_rst_noexp)






