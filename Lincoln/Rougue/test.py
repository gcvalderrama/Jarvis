import glob
import os
from pythonrouge import pythonrouge
from nltk import word_tokenize, WordNetLemmatizer  # , PorterStemmer, LancasterStemmer
import xlsxwriter

ROUGE = './RELEASE-1.5.5/ROUGE-1.5.5.pl'
DATA_PATH = './RELEASE-1.5.5/data'
write_excel = True
workbook = xlsxwriter.Workbook('rouge_test_pres_final.xlsx')


def excel_init(name):
    # Create an new Excel file and add a worksheet.
    if write_excel:
        return workbook.add_worksheet(name)
    else:
        return None


def excel_add_value(row, column, value, worksheet):
    if write_excel: worksheet.write(row, column, value)


def excel_close():
    if write_excel or workbook: workbook.close()


def split_summaries():
    summaries_path = './trial_summaries/*summaries'
    res_path = './trial_summaries/'
    key = "<SUM TYPE=PERDOC SIZE=100 DOCREF="

    for filename in glob.iglob(summaries_path, recursive=True):
        name = os.path.basename(os.path.normpath(filename))
        initial = name.split('_')[0]
        with open(filename, 'r') as file:
            summaries_content = file.read()
        summaries = summaries_content.split(key)
        for summary in summaries:
            if len(summary) > 1:
                parts = summary.split('>')
                with open(res_path + initial + '_' + parts[0].replace(' ', '') + '.txt', 'w') as file:
                    file.write(parts[1].replace('</SUM', ''))


def lemmatizing_files():
    search_path = './trial_summaries/*.txt'
    result_path = './peer_summaries/'

    for filename in glob.iglob(search_path, recursive=True):

        name = os.path.basename(os.path.normpath(filename))

        fx = open(filename, "r")
        summary_content = fx.read()
        fx.close()

        tokens = word_tokenize(summary_content)

        wordnet = WordNetLemmatizer()
        wordnet_text = ''
        for t in tokens:
            wordnet_text += wordnet.lemmatize(t) + ' '
        with open(result_path + name, 'w') as file:
            file.write(wordnet_text)

            # porter = PorterStemmer()
            # porter_text = ''
            # for t in tokens:
            #     porter_text += porter.stem(t) + ' '
            # with open(result_path + 'porter_' + name, 'w') as file:
            #     file.write(porter_text)

            # lancaster = LancasterStemmer()
            # lancaster_text = ''
            # for t in tokens:
            #     lancaster_text += lancaster.stem(t) + ' '
            # with open(result_path + 'lancaster_' + name, 'w') as file:
            #     file.write(lancaster_text)
            #
            # snowball = SnowballStemmer('english')
            # snowball_text = ''
            # for t in tokens:
            #     snowball_text += snowball.stem(t) + ' '
            # with open(result_path + 'snowball_' + name, 'w') as file:
            #     file.write(snowball_text)


def rouge_score_files(summaries_path, text):
    peer_summaries_path = 'D:/Tesis2016/Jarvis/Lincoln/LAB/ManualSummaries/*{0}'
    row = 0

    excel = excel_init(text)
    excel_add_value(0, 0, 'Persona', excel)
    excel_add_value(0, 1, 'Documento', excel)

    for filename in glob.iglob(summaries_path, recursive=True):
        name = os.path.basename(os.path.normpath(filename))
        with open(filename, 'r') as file:
            rst_content = file.read()
        print('Scores for ', name, ' with :')
        for model_file_path in glob.iglob(peer_summaries_path.format(name), recursive=True):
            row += 1
            col = 1
            model_name = os.path.basename(os.path.normpath(model_file_path))

            excel_add_value(row, 0, model_name.split('_')[0], excel)
            excel_add_value(row, 1, name, excel)

            with open(model_file_path, 'r') as file:
                model_content = file.read()
            print(model_name)
            score = pythonrouge.pythonrouge(rst_content, model_content, ROUGE, DATA_PATH)
            for rouge_key, rouge_value in score.items():
                col += 1
                if row == 1:
                    excel_add_value(0, col, rouge_key, excel)
                excel_add_value(row, col, rouge_value, excel)
            print(text, score)


def rouge_v2(templateDir, targetDir, excelName):
    row = 0
    excel = excel_init(excelName)
    excel_add_value(0, 0, 'Documento', excel)
    excel_add_value(0, 1, 'ROUGE-1', excel)
    excel_add_value(0, 2, 'ROUGE-2', excel)
    excel_add_value(0, 3, 'ROUGE-3', excel)
    excel_add_value(0, 4, 'ROUGE-L', excel)
    excel_add_value(0, 5, 'ROUGE-SU4', excel)
    for filename in glob.iglob(templateDir, recursive=True):
        name = os.path.basename(os.path.normpath(filename))
        with open(filename, 'r') as file:
            template_content = file.read()
        with open(targetDir + name, 'r') as file:
            target_content = file.read()
        score = pythonrouge.pythonrouge(template_content, target_content, ROUGE, DATA_PATH)

        row += 1
        col = 0
        model_name = os.path.basename(os.path.normpath(filename))
        excel_add_value(row, 0, model_name, excel)
        print(model_name)
        for rouge_key, rouge_value in score.items():
            if rouge_key == 'ROUGE-1':
                col = 1
            elif rouge_key == 'ROUGE-2':
                col = 2
            elif rouge_key == 'ROUGE-3':
                col = 3
            elif rouge_key == 'ROUGE-L':
                col = 4
            elif rouge_key == 'ROUGE-SU4':
                col = 5
            else:
                raise 'pronblem no column'
            excel_add_value(row, col, rouge_value, excel)



def rouge_files_test():
    # ROUGE = './RELEASE-1.5.5/ROUGE-1.5.5.pl'
    # data_path = './RELEASE-1.5.5/data'

    with open('/Users/ozo/Projects/moulinRouge/generated_summaries/rst/WSJ900402-0112.txt', 'r') as file:
        rst_content = file.read()

    with open('/Users/ozo/Projects/moulinRouge/generated_summaries/conceptual_graph/WSJ900402-0112.txt', 'r') as file:
        graph_content = file.read()

    with open('/Users/ozo/Projects/moulinRouge/peer_summaries/d_summary_001.txt', 'r') as file:
        model_content = file.read()

    with open('/Users/ozo/Projects/Jarvis/Lincoln/07ConceptualGraph/SummariesNonRST/WSJ900402-0112.txt', 'r') as file:
        non_rst_content = file.read()

    score = pythonrouge.pythonrouge(rst_content, model_content, ROUGE, DATA_PATH)
    print('Manual con RST:', score)

    score = pythonrouge.pythonrouge(graph_content, model_content, ROUGE, DATA_PATH)
    print('Manual con Grafos:', score)

    score = pythonrouge.pythonrouge(non_rst_content, model_content, ROUGE, DATA_PATH)
    print('Manual con Non-RST:', score)

    score = pythonrouge.pythonrouge(model_content, non_rst_content, ROUGE, DATA_PATH)
    print('Manual con Non-RST (R):', score)


# split_summaries()
# lemmatizing_files()
write_excel = True

rouge_v2('/Users/gregory/Documents/Github/Jarvis/Final/Test/09Summaries/*.txt',
         '/Users/gregory/Documents/Github/Jarvis/Final/Test/04RSTSummaries/', 'RST')

rouge_v2('/Users/gregory/Documents/Github/Jarvis/Final/Test/09Summaries/*.txt',
         '/Users/gregory/Documents/Github/Jarvis/Final/Test/06ConceptualSummaries/', 'Conceptual')

rouge_v2('/Users/gregory/Documents/Github/Jarvis/Final/Test/09Summaries/*.txt',
         '/Users/gregory/Documents/Github/Jarvis/Final/Test/06ConceptualRSTSummaries/', 'ConceptualRST')

rouge_v2('/Users/gregory/Documents/Github/Jarvis/Final/Test/09Summaries/*.txt',
         '/Users/gregory/Documents/Github/Jarvis/Final/Test/08NLGSummaries/', 'NLG')


if write_excel:
    workbook.close()
